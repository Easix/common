using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Common
{
    /// <summary>
    /// Intel Hex 文件的读取和转换到Bin
    /// </summary>
    public class Hex2Bin
    {
        /*
         * |每行以冒号开头| 数据长度 | 本行数据的起始地址 | 数据类型 |     数据     | 校验和 |
         * |     :       |   1byte |      2byte        |  1byte  |  N data byte |  1byte |
         * 第一个字节:表示本行的数据长度.
         * 第二个,第三个字节表示本行数据的起始地址.
         * 第四字节表示数据类型，数据类型有：0x00、0x01、0x02、0x03、0x04、0x05。
         * '00' Data Rrecord：                     用来记录数据，HEX文件的大部分记录都是数据记录
         * '01' End of File Record:                用来标识文件结束，放在文件的最后，标识HEX文件的结尾
         * '02' Extended Segment Address Record:   用来标识扩展段地址的记录
         * '03' Start Segment Address Record:      开始段地址记录
         * '04' Extended Linear Address Record:    用来标识扩展线性地址的记录 
         *                                         (由于每行标识数据地址的只有2Byte，所以最大只能到64K,
         *                                         为了可以保存高地址的数据,就有了扩展线性地址(高16位)),
         *                                         和开始线性地址组成32位地址)
         * '05' Start Linear Address Record:       开始线性地址记录 (即开始地址(低16位))
         * 然后是数据，最后一个字节 为校验和。
         */
        private enum HexRecordType : byte
        {
            Data = 0x00,
            EndOfFile = 0x01,
            ExtendedSegmentAddress = 0x02,
            StartSegmentAddress = 0x03,
            ExtendedLinearAddress = 0x04,
            StartLinearAddress = 0x05,
            ChecksumError = 0xff,
        }

        private List<byte> hexdata = new List<byte>();
        /// <summary>
        /// Hex文件里的开始地址(偏移量地址)
        /// </summary>
        public Int32 StartAddress {get;set;}
        /// <summary>
        /// 获取整个Hex里的数据
        /// </summary>
        public List<byte> HexData { get { return hexdata; } }
        /// <summary>
        /// 得到Hex里的数据
        /// </summary>
        public byte[] HexBytes { get { return hexdata.ToArray(); } }
        /// <summary>
        /// 获取或设置填充空白的数据
        /// </summary>
        public byte FillEmptyValue { get; set; }
        /// <summary>
        /// 类构造函数
        /// </summary>
        public Hex2Bin() { StartAddress = 0; }
        /// <summary>
        /// 类构造函数(载入Hex,Bin文件)
        /// </summary>
        /// <param name="path">载入文件完整路径</param>
        /// <param name="fillValue">填充空白的数据(默认0xFF,对Bin文件无效)</param>
        /// <param name="startAddress">偏移量地址(默认0,对Hex文件无效)</param>
        public Hex2Bin(string path,byte fillValue = 0xff,Int32 startAddress = 0)
        {
            if(Path.GetExtension(path).ToLower() == ".bin")
            {
                StartAddress = startAddress;
                hexdata.AddRange(File.ReadAllBytes(path));
                return;
            }
            if(Path.GetExtension(path).ToLower() != ".hex") return;
            StreamReader hexStream = File.OpenText(path);
            string hexStrLine;
            HexRecordType hexType;
            FillEmptyValue = fillValue;
            Int32 lineAddr = 0;        /* Hex每行数据的储地址 */
            byte[] lineData = null;    /* Hex每行数据 */
            bool noeAddr = false;
            while((hexStrLine = hexStream.ReadLine()) != null)
            {
                if(hexStrLine.StartsWith(":") == false) { MessageBox.Show("Hex 文件格式错误"); return; }
                hexType = GatHexLineData(hexStrLine, ref lineAddr, out lineData);
                if(hexType == HexRecordType.ChecksumError) { MessageBox.Show("Checksum Error"); return; }
                if(hexType == HexRecordType.Data)
                {
                    if(noeAddr == false) { StartAddress = lineAddr;noeAddr = true; }
                    if(lineAddr < StartAddress)
                    {
                        hexdata.InsertRange(0, Enumerable.Repeat<byte>(FillEmptyValue, (StartAddress - lineAddr)));
                        StartAddress = lineAddr;
                    }
                    if((lineAddr + lineData.Length)- StartAddress > hexdata.Count)
                    {
                        hexdata.AddRange(Enumerable.Repeat<byte>(FillEmptyValue, ((lineAddr + lineData.Length) - StartAddress) - hexdata.Count));
                    }
                    for(byte i = 0; i < lineData.Length; i++) hexdata[(lineAddr- StartAddress) + i] = lineData[i];
                }
            }
        }
        /// <summary>
        /// 类构造函数(载入Byte[])
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startAddress"></param>
        public Hex2Bin(byte[] bytes,Int32 startAddress = 0)
        {
            StartAddress = startAddress;
            hexdata.AddRange(bytes);
        }
        /// <summary>
        /// 计算获取校验和
        /// </summary>
        /// <returns></returns>
        public Int32 GetCheckSum()
        {
            Int32 sum = 0;
            for(Int32 i = 0; i < HexData.Count; i++)
            {
                sum += HexData[i];
            }
            return (sum & 0xffff);            
        }
        /// <summary>
        /// 保存为Hex格式文件
        /// </summary>
        /// <param name="fileName">保存文件路径</param>
        public void SaveAsHex(string fileName)
        {
            SaveToHexFormat(StartAddress, HexBytes, fileName);
        }
        /// <summary>
        /// 保存为Bin格式文件
        /// </summary>
        /// <param name="fileName">保存文件路径</param>
        public void SaveAsBin(string fileName)
        {
            File.WriteAllBytes(fileName, HexBytes);
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="ShowMessageBox"></param>
        public void SaveFileAsDialog(bool ShowMessageBox = false)
        {
            SaveFileDialog fsd = new SaveFileDialog();
            fsd.Filter = "Intel HEX File Format|*.hex|Binary File Format|*.bin";
            if(fsd.ShowDialog() == DialogResult.OK)
            {
                if(fsd.FilterIndex == 1) //hex format
                {
                    SaveAsHex(fsd.FileName);
                }
                else//bin format
                {
                    SaveAsBin(fsd.FileName);
                }
                if(ShowMessageBox)
                {
                    MessageBox.Show("File save OK");
                }
            }
        }
        /// <summary>
        /// 与其它数据合并到自身，如果合并地址有重叠部分，新数据将覆盖老数据
        /// </summary>
        /// <param name="hexbin">合并的数据对象</param>
        /// <param name="fillValue">要填充空白的数据</param>
        public void Merge(Hex2Bin hexbin,byte fillValue)
        {
            if(hexbin == null) return;
            Int32 extAddr = hexbin.StartAddress;
            List<byte> extHexData = hexbin.HexData;
            FillEmptyValue = fillValue;

            if(extAddr < StartAddress)
            {
                /* 如果合并后数据过大(32MByte)则返回 */
                if((StartAddress - extAddr) > 1024000 * 32) { MessageBox.Show("Error：合并后数据大于32MByte"); return; }
                hexdata.InsertRange(0, Enumerable.Repeat<byte>(FillEmptyValue, (StartAddress - extAddr)));
                StartAddress = extAddr;
            }
            else {
                /* 如果合并后数据过大(32MByte)则返回 */
                if((extAddr - StartAddress) > 1024000 * 32) { MessageBox.Show("Error：合并后数据大于32MByte"); return; }
            }
            if((extAddr + extHexData.Count) - StartAddress > hexdata.Count)
            {
                hexdata.AddRange(Enumerable.Repeat<byte>(FillEmptyValue, ((extAddr + extHexData.Count) - StartAddress) - hexdata.Count));
            }
            for(int i = 0; i < extHexData.Count; i++) hexdata[(extAddr - StartAddress) + i] = extHexData[i];
        }
        /// <summary>
        /// Binary 文件保存成Hex文件格式
        /// </summary>
        /// <param name="address">指定数据地址</param>
        /// <param name="bytes">BIN数据组</param>
        /// <param name="fileName">完整文件路径名</param>
        private void SaveToHexFormat(Int32 address,byte[] bytes,string fileName)
        {
            int exaddr = address >>16;
            int low16BitAddr = address & 0x0000ffff;
            int count = 0,haddr = 0;
            List<byte> hexLine = new List<byte>();
            using(StreamWriter sw = File.CreateText(fileName))
            {
                hexLine.Clear();
                if(exaddr > 0)
                {
                    hexLine.AddRange(new byte[] { 0x02, 0x00, 0x00, (byte)HexRecordType.ExtendedLinearAddress });
                    hexLine.Add((byte)(exaddr >> 8));
                    hexLine.Add((byte)exaddr);
                    byte cheksum = 0;
                    for(int i = 0; i < hexLine.Count; i++)
                        cheksum += hexLine[i];
                    hexLine.Add((byte)((~cheksum) + 1));
                    sw.Write(':');
                    sw.WriteLine(hexc.ToHexString(hexLine.ToArray()));
                }
                while(count < bytes.Length)
                {
                    hexLine.Clear();
                    if((count + low16BitAddr & 0x7fff0000) != haddr)
                    {
                        haddr = (count + low16BitAddr & 0x7fff0000);
                        low16BitAddr = 0;
                        exaddr ++;
                        hexLine.AddRange(new byte[] { 0x02, 0x00, 0x00, (byte)HexRecordType.ExtendedLinearAddress });
                        hexLine.Add((byte)(exaddr >> 8));
                        hexLine.Add((byte)exaddr);
                        byte cheksum = 0;
                        for(int i = 0; i < hexLine.Count; i++)
                            cheksum += hexLine[i];
                        hexLine.Add((byte)((~cheksum) + 1));
                    }
                    else
                    {
                        if(bytes.Length - count >= 16)
                             hexLine.Add(0x10);
                        else hexLine.Add((byte)(bytes.Length - count));
                        hexLine.Add((byte)((low16BitAddr + count) >> 8));
                        hexLine.Add((byte)((low16BitAddr + count)));
                        hexLine.Add((byte)HexRecordType.Data);
                        for(int i = 0; i < hexLine[0]; i++)
                            hexLine.Add(bytes[count++]);
                        byte cheksum = 0;
                        for(int i = 0; i < hexLine.Count; i++)
                            cheksum += hexLine[i];
                        hexLine.Add((byte)((~cheksum) + 1));
                    }
                    sw.Write(':');
                    sw.WriteLine(hexc.ToHexString(hexLine.ToArray()));
                }
                sw.Write(":00000001FF");
            }
        }
        Hexc hexc = new Hexc();
        private Int32 baseAddr = 0; /* 扩展基地址 */
        /// <summary>
        /// 解析Hex一行文本数据
        /// </summary>
        /// <param name="hexLine">Hex一行文本数据</param>
        /// <param name="lineAddress">输入\输出数据所在的地址</param>
        /// <param name="bytes">输出数据</param>
        /// <returns>返回解析结果类型</returns>
        private HexRecordType GatHexLineData(string hexLine,ref Int32 lineAddress,out byte[] bytes)
        {
            bytes = null;
            byte[] lineBytes = hexc.HexStringToBytes(hexLine);
            byte count = lineBytes[0];  
            HexRecordType type = (HexRecordType)lineBytes[3];
            if(LineChecksum(lineBytes) == false) { return HexRecordType.ChecksumError; }
            //Int32 baseAddr = 0;

            switch(type)
            {
                case HexRecordType.Data:
                    lineAddress = baseAddr;
                    lineAddress += ((lineBytes[1] << 8) | lineBytes[2]);
                    bytes = new byte[count];
                    for(byte i = 0; i < count; i++) bytes[i] = lineBytes[4 + i];
                    break;
                case HexRecordType.EndOfFile:
                    
                    break;
                case HexRecordType.ExtendedSegmentAddress:                      /* 扩展段地址 */
                    baseAddr = ((lineBytes[4] << 8) | lineBytes[5]) << 4;
                    break;
                case HexRecordType.StartSegmentAddress:                         /* 开始段地址 32位 */
                    baseAddr = ((lineBytes[4] << 24) | (lineBytes[5] << 16) | (lineBytes[6] << 8) | lineBytes[7]);
                    break;
                case HexRecordType.ExtendedLinearAddress:
                    baseAddr = ((lineBytes[4] << 8) | lineBytes[5]) << 16;      /* 扩展线性地址高16(16-31位)位数据地址 */
                    break;
                case HexRecordType.StartLinearAddress:                          /* 开始线性地址32位 */
                    baseAddr = ((lineBytes[4] << 24) | (lineBytes[5] << 16) | (lineBytes[6] << 8) | lineBytes[7]);
                    break;
                default:
                    break;
            }
            return type;
        }
        /* 检查Hex一行数据的校验和是否正确 */
        private bool LineChecksum(byte[] bytes)
        {
            byte b=0;
            int i;
            for(i=0;i+1 < bytes.Length;i++)
            {
                b += bytes[i];
            }
            if((byte)((~b)+1) != bytes[i]) return false;
            return true;
        }
    }
    /// <summary>
    /// HexString to Bytes
    /// </summary>
    public class Hexc
    {

        /// <summary>
        /// 十六进制形式的字符串转换成Byte数组 字符里只能有 0-9 a-f A-F的字符 否则转换失败返回False
        /// //例如 "ac0125"==>0xac 0x01 0x25
        /// </summary>
        /// <param name="instring">要转换十六进制形式的字符串</param>
        /// <param name="buff">输出输出缓冲</param>
        /// <returns>转换失败返回False</returns>
        public bool HexStringToBytes(string instring, out byte[] buff)
        {
            int len = instring.Replace(" ", "").Length;//将字符串里的空格去除
            instring = instring.Trim(" \t\r\n".ToCharArray());//将字符串里前导和尾部的 空格,回车等 去除

            char[] values = instring.ToCharArray();//将字符串复制到数组
            buff = new byte[(len / 2) + (len % 2)];
            char temp;
            int i = 0;
            for(int t = 0; t < values.Length; t++)
            {
                if(values[t] == ' ' || values[t] == '\r' || values[t] == '\n') continue;//如果有空格,回车和换行则进入下一次循环
                temp = values[t];
                if(ConvertHexChr(ref temp)) buff[i] |= (byte)temp;//获取转换好的一个十六进制码
                else { return false; }//{ MessageBox.Show("只能输入 0-9 a-f 的字符", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); return; }
                if((t + 1) >= values.Length || values[t + 1] == ' ' || values[t + 1] == '\r' || values[t + 1] == '\n') { t++; i++; continue; }//如果有空格,回车和换行则进入下一次循环

                temp = values[t + 1];
                if(ConvertHexChr(ref temp))
                {
                    buff[i] <<= 4;//左移4位 如果是上次获得的十六进制码 则用于合并成一个字节
                    buff[i] |= (byte)temp;
                    t++;
                    i++;
                }
                else { return false; }//{MessageBox.Show("只能输入 0-9 a-f 的字符", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); return; }
            }
            return true;
        }
        /// <summary>
        /// 十六进制形式的字符串转换成Byte数组 字符里可以任意字符 但非0-9 a-f A-F的字符将跳过转换
        /// 例如 "ac..0;1q w!@#25"==>0xac 0x01 0x25
        /// </summary>
        /// <param name="HexString">要转换的字符</param>
        /// <returns></returns>
        public byte[] HexStringToBytes(string HexString)
        {
            char[] values = HexString.ToCharArray();//将字符串复制到数组    
            char temp;
            int i = 0;
            for(int t = 0; t < values.Length;)
            {
                temp = values[t++];
                if(ConvertHexChr(ref temp) == false) { continue; }
                i++;t++;
            }
            byte[] buff = new byte[i];
            i = 0;
            for(int t = 0; t < values.Length;)
            {
                temp = values[t++];
                if(ConvertHexChr(ref temp) == false) { continue; }
                buff[i] |= (byte)temp;//获取转换好的一个十六进制码
                if(t >= values.Length) { i++; continue; }
                temp = values[t++];
                if(ConvertHexChr(ref temp) == false) { i++; continue; }
                buff[i] <<= 4;        //左移4位 如果是上次获得的十六进制码 则用于合并成一个字节
                buff[i] |= (byte)temp;
                i++;
            }
            return buff;
        }

        /// <summary>
        /// 字符表示的十六进制数转化为相应的整数,错误则返回  false
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public bool ConvertHexChr(ref char chr)
        {
            if('0' <= chr && chr <= '9') { chr -= '0'; return true; }
            else if('a' <= chr && chr <= 'f') { chr -= 'a'; chr += (char)10; return true; }
            else if('A' <= chr && chr <= 'F') { chr -= 'A'; chr += (char)10; return true; }
            else { return false; }
        }
        /// <summary>
        /// 把字节型转换成十六进制字符串 例如 0xae00cf => "AE00CF"
        /// </summary>
        /// <param name="InBytes">输入Bytes</param>
        /// <param name="sf">填充前缀</param>
        public string ToHexString(byte[] InBytes, string sf = "")
        {
            string hexString = "";
            StringBuilder strB = new StringBuilder();
            foreach(byte InByte in InBytes)
            {
                strB.Append(String.Format("{1}{0:X2}", InByte, sf));
            }
            hexString = strB.ToString();
            return hexString;
        }

        /// <summary>
        /// 转换为十六进制字符串(例如 0xae00cf => "AE00CF")
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public string HexStr<T>(T val)
        {
            string hexString;
            hexString = String.Format("{0:X2}", val);
            return hexString;
        }
    }
    /// <summary>
    /// 循环移位类
    /// </summary>
    public static class Rolr
    {
        /// <summary>
        /// 32位无符号整型循环右移位
        /// </summary>
        /// <param name="value"></param>
        /// <param name="n"></param>
        public static void ro_right(ref uint value, int n)
        {
            value = (value >> n) | (value << (32 - n));
        }
        /// <summary>
        /// 32位无符号整型循环左移位
        /// </summary>
        /// <param name="value"></param>
        /// <param name="n"></param>
        public static void ro_left(ref uint value, int n)
        {
            value = (value >> (32 - n) | (value << n));
        }
    }
}
