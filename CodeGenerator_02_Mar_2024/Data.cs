using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;


enum Analog { AOA_LEFT, AOA_RIGHT }
enum Arinc { ADA_TRUE }

namespace CodeGenerator
{
    public abstract class data_record
    {
        public string type;
        public int interface_num;
        public string SID;
        public string param_name;
        public int no_of_A1 = 0;
        public int no_of_D1 = 0;
        public int size_of_A1 = 0;
        public int size_of_D1 = 0;
        public static int seq_no = 0;
        public static int system_Id = 6;
        public static int port_no = 10;
        public string cell_1, cell_3, cell_4;
        public int cell_2, cell_5, cell_3_A0, cell_4_A0, cell_6, cell_7;
        public string[] Analog_params = { "AoA_loc_left", "AoA_loc_right" };
        public string[] Arinc_params = { "AoA_true" };
    }
    class A0 : data_record
    {
        public A0(Tree AST)
        {
            cell_1 = "A0";
            foreach (Symbol sym in AST.SymbolTable.Values)
                if (sym.Type == DataType.IDENTIFIER)
                    no_of_A1++;

            cell_2 = no_of_A1;

            foreach (string row in AST.retStrList)
                no_of_D1++;

            cell_3_A0 = 78;

            cell_4_A0 = no_of_D1;

            cell_5 = 14;
        }
    }

    class A1 : data_record
    {
        public string getSID(Symbol sym)
        {
            if (sym.name == "AoA_loc_left")
                SID = "A2.2";
            else if (sym.name == "AoA_loc_right")
                SID = "A2.1";
            else if (sym.name == "AoA_true")
                SID = "D4.1";

            return SID;
        }
        public int getInterface(Symbol sym)
        {
            foreach (string param in Analog_params)
            {
                if (param == sym.name)
                    interface_num = 3;
            }
            foreach (string param in Arinc_params)
            {
                if (param == sym.name)
                    interface_num = 4;
            }
            return interface_num;
        }

        public A1(Symbol sym)
        {
            if (sym.Type == DataType.IDENTIFIER)

            {
                cell_1 = "A1";
                cell_2 = getInterface(sym);
                cell_3 = getSID(sym);
                cell_4 = sym.name;
                seq_no++;
                cell_5 = seq_no;
                cell_6 = system_Id;
                cell_7 = port_no;
                system_Id++;
                port_no++;
            }
        }
    }
    class D1 : data_record
    {
        public D1(Tree AST)

        {
            for (int i = 0; i < AST.records; i++)
            {
                // Datalist[1]
            }
            cell_1 = "D0";
        }
    }
    class data
    {
        public string start;
        public int channelId;
        public int header_rowsize;
        public int data_table_size;

        public int data_row_size;

        public static int index = 0;
        public int[] datarow;

        public string[] num;

        public List<string[]> values = new List<string[]>();

        public static data_record[] data_table;

        public data(int header_rowsize, int data_table_size, int data_row_size)

        {
            data_table = new data_record[data_table_size];
        }

        public void add_row(A0 A0_row)
        {
            data_table[index] = A0_row;
            index++;
        }

        public void add_row(A1 A1_row)
        {
            data_table[index] = A1_row;
            index++;
        }

        public void Write_to_csv(Tree AST)
        {
            StreamWriter writer = new StreamWriter("Parallel_Inputs.csv");
            foreach (data_record record in data_table)
            {
                if (record == null)
                    break;
                if (record.cell_1 == "A0" )
                {
                    writer.Write(record.cell_1 + ",");
                    writer.Write(record.no_of_A1 + ",");
                    writer.Write(record.cell_3_A0 + ",");
                    writer.Write(record.no_of_D1 + ",");
                    writer.Write(record.cell_5 + "\n");
                }
                else if (record.cell_1 == "A1")
                {

                    writer.Write(record.cell_1 + ",");
                    writer.Write(record.cell_2 + ",");
                    writer.Write(record.cell_3 + ",");
                    writer.Write(record.cell_4 + ",");
                    writer.Write(record.cell_5 + ",");
                    writer.Write(record.cell_6 + ",");
                    writer.Write(record.cell_7 + "\n");
                }
            }

            foreach (string row in AST.retStrList)
            {
                num = row.Split(",");
                values.Add(num);
            }

            foreach (string[] val in values)

            {

                writer.Write($"D1,");
                foreach (string num in val)
                {
                    writer.Write(Convert.ToInt32(num) + ",");
                }
                writer.WriteLine();
            }
            writer.Close();
        }

        public void Write_to_bin(Tree AST)
        {
            string filename = "Inputs.bin";
            using (BinaryWriter binwriter = new BinaryWriter(File.Open(filename, FileMode.Create), Encoding.ASCII))
            {
                foreach (data_record record in data_table)
                {
                    if (record == null)
                        break;
                    if (record.cell_1 == "A0")
                    {
                        binwriter.Write(record.cell_1);
                        binwriter.Write(BitConverter.GetBytes(record.no_of_A1));
                        binwriter.Write(BitConverter.GetBytes(record.cell_3_A0));
                        binwriter.Write(BitConverter.GetBytes(record.no_of_D1));
                        binwriter.Write(BitConverter.GetBytes(record.cell_5));
                    }


                    else if (record.cell_1 == "A1")
                    {
                        binwriter.Write(record.cell_1);
                        binwriter.Write(BitConverter.GetBytes(record.cell_2));
                        binwriter.Write(record.cell_3);
                        binwriter.Write(record.cell_4);
                        binwriter.Write(BitConverter.GetBytes(record.cell_5));
                        binwriter.Write(record.cell_6);
                        binwriter.Write(record.cell_7);
                    }
                }

                foreach (string row in AST.retStrList)
                {
                    num = row.Split(",");
                }

                foreach (string[] row in values)
                {
                    binwriter.Write(new char[] { 'D', '1' });

                    foreach (string num in row)
                    {
                        binwriter.Write(BitConverter.GetBytes(Convert.ToInt32(num)));
                    }
                }
                binwriter.Close();
            }
        }

        public void Read_from_bin()
        {
            string filename = "Inputs.bin";

            using (BinaryReader binreader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                foreach (data_record record in data_table)
                {
                    if (record == null)

                        break;

                    if (record.cell_1 == "Α0")
                    {
                        Console.WriteLine(binreader.Read());
                        Console.WriteLine(binreader.ReadInt32());
                        Console.WriteLine(binreader.ReadInt32());
                        Console.WriteLine(binreader.ReadInt32());
                        Console.WriteLine(binreader.ReadInt32());
                    }
                    else if (record.cell_1 == "A1")
                    {
                        Console.WriteLine(binreader.Read());
                        Console.WriteLine(binreader.ReadInt32());
                        Console.WriteLine(binreader.Read());
                        Console.WriteLine(binreader.Read());
                        Console.WriteLine(binreader.ReadInt32());
                    }
                }
                binreader.Close();
            }
        }

        public data_record[] GenerateDataTable(Tree AST)
        {

            A0 A0_row = new A0(AST);
            add_row(A0_row);

            foreach (Symbol sym in AST.SymbolTable.Values)
            {
                if (sym.Type == DataType.IDENTIFIER)
                   {
                    A1 A1_row = new A1(sym);
                    add_row(A1_row);
                }

            }
            return data_table;
        }
    }
}