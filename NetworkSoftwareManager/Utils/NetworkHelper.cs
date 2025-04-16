using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace NetworkSoftwareManager.Utils
{
    /// <summary>
    /// Helper class for network-related operations.
    /// </summary>
    public static class NetworkHelper
    {
        /// <summary>
        /// Parses a string containing IP ranges and returns a list of IPRange objects.
        /// </summary>
        /// <param name="ipRangeText">
        /// String containing IP ranges in formats like:
        /// - Single IP: 192.168.1.1
        /// - Range: 192.168.1.1-192.168.1.254
        /// - CIDR: 192.168.1.0/24
        /// - Multiple ranges separated by commas or semicolons
        /// </param>
        public static List<IPRange> ParseIPRanges(string ipRangeText)
        {
            var result = new List<IPRange>();
            
            if (string.IsNullOrWhiteSpace(ipRangeText))
            {
                return result;
            }
            
            // Split by comma or semicolon
            var ranges = ipRangeText.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var range in ranges)
            {
                var trimmedRange = range.Trim();
                
                // Check for CIDR notation (e.g., 192.168.1.0/24)
                if (trimmedRange.Contains('/'))
                {
                    var cidrRange = ParseCIDRRange(trimmedRange);
                    if (cidrRange != null)
                    {
                        result.Add(cidrRange);
                    }
                }
                // Check for IP range notation (e.g., 192.168.1.1-192.168.1.254)
                else if (trimmedRange.Contains('-'))
                {
                    var ipRange = ParseIPRange(trimmedRange);
                    if (ipRange != null)
                    {
                        result.Add(ipRange);
                    }
                }
                // Single IP address
                else if (IPAddress.TryParse(trimmedRange, out IPAddress? ip))
                {
                    result.Add(new IPRange(ip, ip));
                }
            }
            
            return result;
        }

        /// <summary>
        /// Parses a CIDR notation string (e.g., 192.168.1.0/24) into an IPRange.
        /// </summary>
        private static IPRange? ParseCIDRRange(string cidrNotation)
        {
            try
            {
                // Split the CIDR notation into IP and prefix length
                var parts = cidrNotation.Split('/');
                if (parts.Length != 2)
                {
                    return null;
                }
                
                if (!IPAddress.TryParse(parts[0], out IPAddress? ip))
                {
                    return null;
                }
                
                if (!int.TryParse(parts[1], out int prefixLength) || prefixLength < 0 || prefixLength > 32)
                {
                    return null;
                }
                
                // Calculate network and broadcast addresses
                byte[] ipBytes = ip.GetAddressBytes();
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(ipBytes);
                }
                
                uint ipUint = BitConverter.ToUInt32(ipBytes, 0);
                uint mask = (prefixLength == 0) ? 0 : ~((1u << (32 - prefixLength)) - 1);
                
                uint networkUint = ipUint & mask;
                uint broadcastUint = networkUint | ~mask;
                
                byte[] networkBytes = BitConverter.GetBytes(networkUint);
                byte[] broadcastBytes = BitConverter.GetBytes(broadcastUint);
                
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(networkBytes);
                    Array.Reverse(broadcastBytes);
                }
                
                IPAddress startIP = new IPAddress(networkBytes);
                IPAddress endIP = new IPAddress(broadcastBytes);
                
                return new IPRange(startIP, endIP);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Parses an IP range string (e.g., 192.168.1.1-192.168.1.254) into an IPRange.
        /// </summary>
        private static IPRange? ParseIPRange(string rangeNotation)
        {
            try
            {
                // Split the range into start and end IPs
                var parts = rangeNotation.Split('-');
                if (parts.Length != 2)
                {
                    return null;
                }
                
                if (!IPAddress.TryParse(parts[0].Trim(), out IPAddress? startIP))
                {
                    return null;
                }
                
                if (!IPAddress.TryParse(parts[1].Trim(), out IPAddress? endIP))
                {
                    return null;
                }
                
                return new IPRange(startIP, endIP);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Validates if a string is a valid IP address.
        /// </summary>
        public static bool IsValidIPAddress(string ipAddress)
        {
            return IPAddress.TryParse(ipAddress, out _);
        }

        /// <summary>
        /// Validates if a string is a valid hostname.
        /// </summary>
        public static bool IsValidHostname(string hostname)
        {
            if (string.IsNullOrWhiteSpace(hostname))
            {
                return false;
            }
            
            // Check length constraints
            if (hostname.Length > 255)
            {
                return false;
            }
            
            // Hostname regex pattern
            var pattern = @"^([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])(\.([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9]))*$";
            return Regex.IsMatch(hostname, pattern);
        }
    }

    /// <summary>
    /// Represents a range of IP addresses.
    /// </summary>
    public class IPRange
    {
        public IPAddress StartIP { get; }
        public IPAddress EndIP { get; }

        public IPRange(IPAddress startIP, IPAddress endIP)
        {
            StartIP = startIP;
            EndIP = endIP;
        }

        /// <summary>
        /// Gets all IP addresses in this range.
        /// </summary>
        public List<string> GetAllIPsInRange()
        {
            var result = new List<string>();
            
            uint start = ConvertToUInt32(StartIP);
            uint end = ConvertToUInt32(EndIP);
            
            if (end < start)
            {
                // Swap if end is less than start
                uint temp = start;
                start = end;
                end = temp;
            }
            
            // Limit the range to prevent excessive memory usage
            uint maxIPs = 65536; // Max 64K IPs
            if (end - start > maxIPs)
            {
                end = start + maxIPs;
            }
            
            for (uint i = start; i <= end; i++)
            {
                result.Add(ConvertToIPAddress(i).ToString());
            }
            
            return result;
        }

        /// <summary>
        /// Converts an IP address to a uint.
        /// </summary>
        private uint ConvertToUInt32(IPAddress ipAddress)
        {
            byte[] bytes = ipAddress.GetAddressBytes();
            
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            
            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// Converts a uint to an IP address.
        /// </summary>
        private IPAddress ConvertToIPAddress(uint ipValue)
        {
            byte[] bytes = BitConverter.GetBytes(ipValue);
            
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            
            return new IPAddress(bytes);
        }

        /// <summary>
        /// Checks if an IP address is within this range.
        /// </summary>
        public bool Contains(IPAddress ipAddress)
        {
            uint ip = ConvertToUInt32(ipAddress);
            uint start = ConvertToUInt32(StartIP);
            uint end = ConvertToUInt32(EndIP);
            
            return ip >= start && ip <= end;
        }

        /// <summary>
        /// Returns a string representation of this IP range.
        /// </summary>
        public override string ToString()
        {
            return $"{StartIP}-{EndIP}";
        }
    }
}
