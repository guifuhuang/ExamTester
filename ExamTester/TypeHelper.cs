using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ExamTester
{
    public static class TypeHelper
    {
        public static void HandleException(this System.Exception ex)
        {
            StackTrace st = new StackTrace(ex);

            if (st.FrameCount > 0)
            {
                LogHelper.Error("{0}:{1}", st.GetFrame(st.FrameCount - 1).GetMethod().Name, ex.Message);
            }
        }
        /// <summary>
        /// 整数验证
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsInteger(this string s)
        {
            string pattern = @"^\d*$";
            return System.Text.RegularExpressions.Regex.IsMatch(s, pattern);
        }

        #region IsNullOrEmptyString
        /// <summary>
        /// Identify whether the string is null or empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyString(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
        #endregion

        #region Convert
        /// <summary>
        /// Safely convert to string 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSafeString(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value;
        }

        /// <summary>
        /// Safely convert to string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSafeString(this object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.ToString();
        }

        /// <summary>
        /// Safely substring
        /// </summary>
        /// <param name="content"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToSafeSubstring(this string content, int startIndex, int length)
        {
            string substringContent = content.ToSafeString();

            if (substringContent.Length >= startIndex + length)
            {
                return substringContent.Substring(startIndex, length);
            }
            else if (substringContent.Length > startIndex)
            {
                return substringContent.Substring(startIndex);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Safely substring
        /// </summary>
        /// <param name="content"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToSafeSubstring(this object content, int startIndex, int length)
        {
            return content.ToSafeString().ToSafeSubstring(startIndex, length);
        }

        /// <summary>
        /// Safely convert to decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToSafeDecimal(this decimal? value)
        {
            return value.HasValue ? value.Value : 0.0m;
        }
        /*
        /// <summary>
        /// Safely convert to decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToSafeDecimal(this OracleDecimal? value)
        {
            return value.ToSafeOracleDecimal().ToSafeDecimal();
        }
        /// <summary>
        /// Safely convert to decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToSafeDecimal(this OracleDecimal value)
        {
            return value.Value;
        }
        */
        /// <summary>
        /// Safely convert to decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToSafeDecimal(this string value)
        {
            decimal rtn = 0.0m;
            if (value == null)
            {
                return rtn;
            }
            if (value.Replace("*", string.Empty).IsNullOrEmptyString())
            {
                return rtn;
            }


            try
            {
                rtn = decimal.Parse(value);
            }
            catch
            {
                rtn = 0.0m;
            }

            return rtn;
        }

        /// <summary>
        /// Safely convert to decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToSafeDecimal(this object value)
        {
            decimal rtn = 0.0m;

            if (value == null)
            {
                return rtn;
            }

            string stringValue = value.ToSafeString().Replace(",", ".").Replace("*", string.Empty);

            try
            {
                if (stringValue.Contains("E") || stringValue.Contains("e"))
                {
                    rtn = Convert.ToDecimal(Decimal.Parse(stringValue.ToString(), System.Globalization.NumberStyles.Float));
                }
                else
                {
                    rtn = decimal.Parse(stringValue);
                }
            }
            catch
            {
                rtn = 0.0m;
            }

            return rtn;
        }

        /*
        /// <summary>
        /// Safely convert to Oracle decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static OracleDecimal ToSafeOracleDecimal(this decimal? value)
        {
            return value.ToSafeDecimal().ToSafeOracleDecimal();
        }
        /// <summary>
        /// Safely convert to Oracle decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static OracleDecimal ToSafeOracleDecimal(this decimal value)
        {
            return value;
        }
        /// <summary>
        /// Safely convert to Oracle decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static OracleDecimal ToSafeOracleDecimal(this OracleDecimal? value)
        {
            return value.HasValue ? value.Value : 0.0m;
        }
        /// <summary>
        /// Safely convert to Oracle decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static OracleDecimal ToSafeOracleDecimal(this string value)
        {
            OracleDecimal rtn = 0.0m;
            if (value.IsNullOrEmptyString())
            {
                return rtn;
            }
            value = value.AdjustNumberSignPostion();
            try
            {
                rtn = OracleDecimal.Parse(value);
            }
            catch
            {
                rtn = 0.0m;
            }
            return rtn;
        }
        /// <summary>
        /// Safely convert to Oracle decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static OracleDecimal? ToSafeOracleDecimalNullable(this string value)
        {
            OracleDecimal? rtn = null;
            if (value.IsNullOrEmptyString())
            {
                return rtn;
            }
            value.AdjustNumberSignPostion();
            try
            {
                rtn = OracleDecimal.Parse(value);
            }
            catch
            {
                rtn = null;
            }
            return rtn;
        }
        /// <summary>
        /// Safely convert to Oracle decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static OracleDecimal ToSafeOracleDecimal(this object value)
        {
            OracleDecimal rtn = 0.0m;
            if (value == null)
            {
                return rtn;
            }
            string stringValue = value.ToSafeString();
            stringValue = stringValue.AdjustNumberSignPostion();
            try
            {
                rtn = OracleDecimal.Parse(stringValue);
            }
            catch
            {
                rtn = 0.0m;
            }
            return rtn;
        }
        */
        /// <summary>
        /// Safely convert to int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToSafeInt(this int? value)
        {
            return value.HasValue ? value.Value : 0;
        }

        /// <summary>
        /// Safely convert to int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToSafeInt(this string value)
        {
            int rtn = 0;

            if (value.IsNullOrEmptyString())
            {
                return rtn;
            }

            try
            {
                rtn = int.Parse(value);
            }
            catch
            {
                rtn = 0;
            }

            return rtn;
        }

        /// <summary>
        /// Safely convert to int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToSafeInt(this object value)
        {
            int rtn = 0;

            if (value == null)
            {
                return rtn;
            }

            string stringValue = value.ToSafeString();

            try
            {
                rtn = int.Parse(stringValue);
            }
            catch
            {
                rtn = 0;
            }

            return rtn;
        }

        /// <summary>
        /// Safely convert to long
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToSafeLong(this long? value)
        {
            return value.HasValue ? value.Value : 0;
        }

        /// <summary>
        /// Safely convert to long
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToSafeLong(this string value)
        {
            long rtn = 0;

            if (value.IsNullOrEmptyString())
            {
                return rtn;
            }


            try
            {
                rtn = long.Parse(value);
            }
            catch
            {
                rtn = 0;
            }

            return rtn;
        }

        /// <summary>
        /// Safely convert to long
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToSafeLong(this object value)
        {
            long rtn = 0;

            if (value == null)
            {
                return rtn;
            }

            string stringValue = value.ToSafeString();

            try
            {
                rtn = long.Parse(stringValue);
            }
            catch
            {
                rtn = 0;
            }

            return rtn;
        }

        /// <summary>
        /// Safely convert to datetime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToSafeDateTime(this string value)
        {
            DateTime rtn = DateTime.MinValue;

            if (value.IsNullOrEmptyString())
            {
                return rtn;
            }

            try
            {
                rtn = DateTime.Parse(value);
            }
            catch
            {
                rtn = DateTime.MinValue;
            }

            return rtn;
        }

        /// <summary>
        /// Safely convert to datetime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToSafeDateTime(this DateTime? value)
        {
            return value.HasValue ? value.Value : DateTime.MinValue;
        }

        /// <summary>
        /// Convert to int display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToIntString(this decimal value)
        {
            return value.ToString("0");
        }

        /// <summary>
        /// Convert to int display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToIntString(this int value)
        {
            return value.ToString("0");
        }

        /// <summary>
        /// Convert to long display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToLongString(this decimal value)
        {
            return value.ToString("0");
        }

        /// <summary>
        /// Convert to long display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToLongString(this long value)
        {
            return value.ToString("0");
        }

        /// <summary>
        /// Convert to decimal display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDecimalString(this decimal value)
        {
            return value.ToDecimalString(2);
        }
        /*
        /// <summary>
        /// Convert to decimal display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDecimalString(this OracleDecimal value)
        {
            return value.ToDecimalString(2);
        }
        */
        /// <summary>
        /// Convert to decimal display string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimalCount">The decimal count of the string</param>
        /// <returns></returns>
        public static string ToDecimalString(this decimal value, int decimalCount)
        {
            return value.ToString("0." + new string('0', decimalCount));
        }
        /*
        /// <summary>
        /// Convert to decimal display string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimalCount">The decimal count of the string</param>
        /// <returns></returns>
        public static string ToDecimalString(this OracleDecimal value, int decimalCount, bool isAmount = false)
        {
            string rtn = value.ToString();
            if (rtn.IndexOf(".") == -1)
            {
                rtn += ".";
            }
            rtn += new string('0', decimalCount);
            rtn = rtn.Substring(0, rtn.IndexOf(".") + decimalCount + 1);
            if (isAmount)
            {
                var beforeSign = "-";
                if (rtn.IndexOf(beforeSign) == -1)
                {
                    beforeSign = string.Empty;
                }
                var substr = string.Empty;
                var strInteger = rtn.Substring(0, rtn.IndexOf("."));
                if (!string.IsNullOrEmpty(beforeSign))
                {
                    strInteger = strInteger.Replace(beforeSign, string.Empty);
                }
                var strDecimal = rtn.Substring(rtn.IndexOf("."));
                while (strInteger.Length > 3)
                {
                    substr = "," + strInteger.Substring(strInteger.Length - 3) + substr;
                    strInteger = strInteger.Substring(0, strInteger.Length - 3);
                }
                substr = strInteger + substr;
                rtn = beforeSign + substr + strDecimal;
            }
            return rtn;
        }
        */
        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToddMMyyyy(this DateTime? value)
        {
            return value.ToSafeDateTime().DateTimeToddMMyyyy();
        }

        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToddMMyyyy(this DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return string.Empty;
            }

            return value.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToddMMyyyyhhmmss(this DateTime? value)
        {
            return value.ToSafeDateTime().DateTimeToddMMyyyyhhmmss();
        }

        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToddMMyyyyhhmmss(this DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return string.Empty;
            }

            return value.ToString("dd/MM/yyyy hh:mm:ss");
        }

        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToyyyyMMdd(this DateTime? value)
        {
            return value.ToSafeDateTime().DateTimeToyyyyMMdd();
        }

        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToyyyyMMdd(this DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return string.Empty;
            }

            return value.ToString("yyyyMMdd");
        }

        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToyyyy_MM_dd(this DateTime? value)
        {
            return value.ToSafeDateTime().DateTimeToyyyy_MM_dd();
        }

        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToyyyy_MM_dd(this DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return string.Empty;
            }

            return value.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToyyyyMMddHHmmss(this DateTime? value)
        {
            return value.ToSafeDateTime().DateTimeToyyyyMMddHHmmss();
        }

        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToyyyyMMddHHmmss(this DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return string.Empty;
            }

            return value.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToyyyy_MM_dd_HH_mm_ss(this DateTime? value)
        {
            return value.ToSafeDateTime().DateTimeToyyyy_MM_dd_HH_mm_ss();
        }

        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToyyyy_MM_dd_HH_mm_ss(this DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return string.Empty;
            }

            return value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Safely convert to datetime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime yyyyMMddToDateTime(this string value)
        {
            try
            {
                return DateTime.ParseExact(value, "yyyyMMdd", null);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Safely convert to datetime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime yyyyMMddHHmmssToDateTime(this string value)
        {
            try
            {
                return DateTime.ParseExact(value, "yyyyMMddHHmmss", null);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Safely convert to datetime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime dd_MM_yyyyToDateTime(this string value)
        {
            try
            {
                return DateTime.ParseExact(value, "dd/MM/yyyy", null);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Safely convert to boolean
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        public static bool ToSafeBool(this string value)
        {
            if (value.IsNullOrEmptyString())
            {
                return false;
            }

            try
            {
                return bool.Parse(value);
            }
            catch
            {
                return false;
            }
        }

        public static object SafeConvert(this object value, Type convertToType)
        {
            object rtn = null;

            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            if (convertToType == typeof(string))
            {
                rtn = Convert.ToString(value);
            }
            else if (convertToType == typeof(int) || convertToType == typeof(Nullable<int>))
            {
                rtn = Convert.ToInt32(value);
            }
            else if (convertToType == typeof(long) || convertToType == typeof(Nullable<long>))
            {
                rtn = Convert.ToInt64(value);
            }
            else if (convertToType == typeof(double) || convertToType == typeof(Nullable<double>))
            {
                rtn = Convert.ToDouble(value);
            }
            else if (convertToType == typeof(float) || convertToType == typeof(Nullable<float>))
            {
                rtn = Convert.ToSingle(value);
            }
            else if (convertToType == typeof(decimal) || convertToType == typeof(Nullable<decimal>))
            {
                rtn = Convert.ToDecimal(value);
            }
            //else if (convertToType == typeof(OracleDecimal) || convertToType == typeof(Nullable<OracleDecimal>))
            //{
            //    if (value is OracleDecimal && ((OracleDecimal)value).IsNull)
            //    {
            //        return null;
            //    }

            //    rtn = OracleDecimal.Parse(Convert.ToString(value));
            //}
            else if (convertToType == typeof(DateTime) || convertToType == typeof(Nullable<DateTime>))
            {
                rtn = Convert.ToDateTime(value);
            }

            return rtn;
        }
        public static double ToSafeDouble(this string value)
        {
            double rtn = 0;
            //if (value.Contains("E") || value.Contains("e"))
            //{
            //    rtn = Convert.ToDouble(Double.Parse(value.ToString(), System.Globalization.NumberStyles.Float));
            //}
            rtn = Convert.ToDouble(Double.Parse(value.ToString(), System.Globalization.NumberStyles.Float));
            return rtn;
        }
        #endregion

        #region FormatString
        /// <summary>
        /// Format string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatString(this string value, List<string> args)
        {
            return value.FormatString(args.ToArray());
        }

        /// <summary>
        /// Format string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatString(this string value, params object[] args)
        {
            List<object> argList = new List<object>();
            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] is List<string>)
                    {
                        argList.AddRange(args[i] as List<string>);
                    }
                    else
                    {
                        argList.Add(args[i]);
                    }
                }

                return string.Format(value, argList.ToArray());
            }
            else
            {
                return value;
            }
        }
        #endregion

        #region Stream Convert
        /// <summary>
        /// Stream convert to string
        /// </summary>
        /// <param name="value">Stream</param>
        /// <returns>String</returns>
        public static string StreamToString(this Stream value)
        {
            return StreamToBytes(value).BytesToString();
        }

        /// <summary>
        /// Stream convert to byte
        /// </summary>
        /// <param name="value">Stream</param>
        /// <returns>Byte</returns>
        public static byte[] StreamToBytes(this Stream value)
        {
            List<byte> rtn = new List<byte>();
            byte[] buffer = new byte[100];
            int offset = 0;
            int totalCount = 0;
            while (true)
            {
                int bytesRead = value.Read(buffer, 0, 100);
                if (bytesRead == 0)
                {
                    break;
                }
                offset += bytesRead;
                totalCount += bytesRead;
                rtn.AddRange(buffer.Take(bytesRead));
            }

            return rtn.ToArray();
        }

        /// <summary>
        /// String convert to byte (UTF8)
        /// </summary>
        /// <param name="value">String</param>
        /// <returns>Byte</returns>
        public static byte[] StringToBytes(this string value)
        {
            byte[] rtn = Encoding.UTF8.GetBytes(value);

            return rtn;
        }

        /// <summary>
        /// Byte convert to string
        /// </summary>
        /// <param name="value">Byte</param>
        /// <returns>String</returns>
        public static string BytesToString(this byte[] value)
        {
            return BytesToString(value, Encoding.UTF8);
        }

        /// <summary>
        /// Byte convert to string
        /// </summary>
        /// <param name="value">Byte</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>String</returns>
        public static string BytesToString(this byte[] value, Encoding encoding)
        {
            string rtn = encoding.GetString(value);

            return rtn;
        }
        #endregion

        #region Trim Words
        /// <summary>
        /// Trim the extra words
        /// </summary>
        /// <param name="content"></param>
        /// <param name="maxWords"></param>
        /// <returns></returns>
        public static string TrimWords(this string content, int maxWords)
        {
            string trimContent = content.ToSafeString();

            if (trimContent.Length > maxWords + 3)
            {
                return trimContent.Substring(0, maxWords - 3) + "...";
            }

            return trimContent;
        }

        /// <summary>
        /// Trim the extra words
        /// </summary>
        /// <param name="content"></param>
        /// <param name="maxWords"></param>
        /// <returns></returns>
        public static string TrimWords(this object content, int maxWords)
        {
            return content.ToSafeString().TrimWords(maxWords);
        }
        #endregion

        #region Convert type
        /// <summary>
        /// Convert the type from object
        /// </summary>
        /// <typeparam name="NewType">The type need convert to</typeparam>
        /// <param name="obj">The object want to convert</param>
        /// <returns>New type of object</returns>
        public static NewType ConvertType<NewType>(this object obj) where NewType : new()
        {
            if (!(obj is NewType))
            {
                throw new System.Exception("Special type can not be converted successfully");
            }

            return (NewType)obj;
        }
        #endregion Convert type

        /// <summary>
        /// 拷贝对象的相同属性值
        /// </summary>
        /// <param name="objB"></param>
        /// <param name="objA"></param>
        public static void CopySamePropertysFrom(this object objB, object objA)
        {
            List<System.Reflection.PropertyInfo> lstProA = objA.GetType().GetProperties().ToList();
            List<System.Reflection.PropertyInfo> lstProB = objB.GetType().GetProperties().ToList();
            foreach (System.Reflection.PropertyInfo proA in lstProA)
            {
                if (objB.GetType().GetProperty(proA.Name) != null)
                {
                    System.Reflection.PropertyInfo proB = objB.GetType().GetProperty(proA.Name);
                    if (proB.CanWrite)
                    {
                        proB.SetValue(objB, proA.GetValue(objA, null), null);
                    }
                }
            }
        }

        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToyyyy_MM_dd_HH_mm_ss_fff(this DateTime? value)
        {
            return value.ToSafeDateTime().DateTimeToyyyy_MM_dd_HH_mm_ss_fff();
        }

        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToyyyy_MM_dd_HH_mm_ss_fff(this DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return string.Empty;
            }
            string sDtMsg = value.DateTimeToyyyyMMddHHmmssfff();

            return string.Format("{0}-{1}-{2} {3}:{4}:{5}.{6}"
            , sDtMsg.Substring(0, 4) // 年
            , sDtMsg.Substring(4, 2) // 月
            , sDtMsg.Substring(6, 2) // 日
            , sDtMsg.Substring(8, 2) // 时
            , sDtMsg.Substring(10, 2) // 分
            , sDtMsg.Substring(12, 2) // 秒
            , sDtMsg.Substring(14));
        }

        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToyyyyMMddHHmmssfff(this DateTime? value)
        {
            return value.ToSafeDateTime().DateTimeToyyyyMMddHHmmssfff();
        }

        /// <summary>
        /// Convert to datetime display string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DateTimeToyyyyMMddHHmmssfff(this DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return string.Empty;
            }
            StringBuilder sbTime = new StringBuilder(value.DateTimeToyyyyMMddHHmmss());
            if (value.TimeOfDay.ToString().Length < 9)
            {
                sbTime.Append("000");
            }
            else
            {
                sbTime.Append(value.TimeOfDay.ToString().Remove(0, 9));
            }
            return sbTime.ToString();
        }
        /// <summary>
        /// 将yyyyMMddHHmmssfff格式化成yyyy-MM-dd HH:mm:ss.ffff格式
        /// </summary>
        /// <param name="sDateTimeString">yyyyMMddHHmmssfff字符串</param>
        /// <returns>yyyy-MM-dd HH:mm:ss.ffff格式字符串</returns>
        public static string FormatDateTimeStingToOracleTimeStampString(this string sDateTimeString)
        {
            string sOracleTimeStampString = string.Empty;
            // 时间戳
            if (!string.IsNullOrWhiteSpace(sDateTimeString))
            {
                DateTime dtNow = System.DateTime.Now;
                if (DateTime.TryParse(sDateTimeString, out dtNow))
                {
                    DateTime dtMsg = DateTime.Parse(sDateTimeString);
                    sOracleTimeStampString = TypeHelper.DateTimeToyyyy_MM_dd_HH_mm_ss_fff(dtMsg);
                }
                else
                {
                    if (sDateTimeString.Length > 14)
                    {
                        sOracleTimeStampString = string.Format("{0}-{1}-{2} {3}:{4}:{5}.{6}"
                            , sDateTimeString.Substring(0, 4) // 年
                            , sDateTimeString.Substring(4, 2) // 月
                            , sDateTimeString.Substring(6, 2) // 日
                            , sDateTimeString.Substring(8, 2) // 时
                            , sDateTimeString.Substring(10, 2) // 分
                            , sDateTimeString.Substring(12, 2) // 秒
                            , sDateTimeString.Substring(14));
                        if (!DateTime.TryParse(sDateTimeString, out dtNow))
                        {
                            DateTime dtNowTime = System.DateTime.Now;
                            sOracleTimeStampString = TypeHelper.DateTimeToyyyy_MM_dd_HH_mm_ss_fff(dtNowTime);
                        }
                    }
                    else
                    {
                        DateTime dtNowTime = System.DateTime.Now;
                        sOracleTimeStampString = TypeHelper.DateTimeToyyyy_MM_dd_HH_mm_ss_fff(dtNowTime);
                    }
                }
            }
            else
            {
                DateTime dtNowTime = System.DateTime.Now;
                sOracleTimeStampString = TypeHelper.DateTimeToyyyy_MM_dd_HH_mm_ss_fff(dtNowTime);
            }
            return sOracleTimeStampString;
        }
        /// <summary>
        /// 按字节截取字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string CutStringByte(string str, int startIndex, int len)
        {
            if (str == null || str.Trim() == "")
            {
                return "";
            }
            if (Encoding.Default.GetByteCount(str) < startIndex + 1)
            {
                return string.Empty;
            }
            int i = 0;//字节数
            int j = 0;//实际截取长度
            int iCount = 0;// 实际的起始位置
            foreach (char newChar in str)
            {
                if ((int)newChar > 127)
                {
                    //汉字
                    i += 2;
                }
                else
                {
                    i++;
                }
                iCount++;
                if (i > startIndex + len)
                {
                    int iStart = startIndex - i + iCount;
                    if (iStart <= 0 || startIndex == 0)
                    {
                        iStart = 0;
                    }
                    str = str.Substring(iStart, j);
                    break;
                }
                if (i > startIndex)
                {
                    j++;
                }
            }
            return str;
        }
        public static string CutStringByte(string str, int startIndex)
        {
            if (str == null || str.Trim() == "")
            {
                return "";
            }
            if (Encoding.Default.GetByteCount(str) < startIndex + 1)
            {
                return string.Empty;
            }
            int i = 0;//字节数
            //int j = 0;//实际截取长度
            int iCount = 0;// 实际的起始位置
            foreach (char newChar in str)
            {
                if ((int)newChar > 127)
                {
                    //汉字
                    i += 2;
                }
                else
                {
                    i++;
                }
                iCount++;
                if (i > startIndex)
                {
                    int iStart = startIndex - i + iCount;
                    if (iStart <= 0 || startIndex == 0)
                    {
                        iStart = 0;
                    }
                    str = str.Substring(iStart);
                    break;
                }
                //if (i > startIndex)
                //{
                //    j++;
                //}
            }
            return str;
        }
    }
}
