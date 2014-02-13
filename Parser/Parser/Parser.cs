﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    static class Parser
    {
        static string path = "S:\\Bitcoin\\Blockchain\\blocks\\";
        static bool shouldExit = false;
        static public void Parse()
        {
            int fileIndex = 0;
            while (!shouldExit)
            {
                
                string fileName = path+getFileName(fileIndex);
                if (File.Exists(fileName))
                {
                    byte[] currentFile = File.ReadAllBytes(fileName);
                    //List<uint> blockIndexes = buildIndex(ref currentFile);
                    parseFileDataIntoClasses(ref currentFile);
                    fileIndex++;
                    Console.WriteLine("Finished proccessing file " + fileName);
                }
                else
                {
                    shouldExit = true;
                    Console.WriteLine(ScryptExtraction.counter.ToString());
                    Console.WriteLine("Done.");
                    Console.ReadLine();
                }
            }
        }
        private static string getFileName(int fileIndex)
        {
            if (fileIndex <= 99999)
            {
                string fileName = "blk";
                string fileNumber = fileIndex.ToString();
                for (int i = fileNumber.Length; i < 5; i++)
                {
                    fileName += "0";
                }
                fileName += fileNumber;
                fileName += ".dat";
                return fileName;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static bool seekToNextHeader(ref uint cursor, ref byte[] currentFile)
        {
            while (cursor + 3 < currentFile.Length)
            {
                if (currentFile[cursor] == 249)
                {
                    if (currentFile[cursor + 1] == 190)
                    {
                        if (currentFile[cursor + 2] == 180)
                        {
                            if (currentFile[cursor + 3] == 217)
                            {
                                return true;
                            }
                        }
                    }
                }
                cursor++;
            }
            return false;
        }

        private static byte[] getBlockData(ref uint cursor, ref byte[] currentFile)
        {
            uint header = parseFourBytesToElement(ref cursor, ref currentFile);
            uint blockDataLength = parseFourBytesToElement(ref cursor, ref currentFile);
            if (cursor + blockDataLength < currentFile.Length)
            {
                byte[] block = new byte[blockDataLength];
                Array.Copy(currentFile, cursor, block, 0, blockDataLength);
                return block;
            }
            return null;
        }
        private static void parseFileDataIntoClasses(ref byte[] currentFile)
        {
            uint fileCursor = 0;
            while(seekToNextHeader(ref fileCursor, ref currentFile))
            {
                byte[] blockData = getBlockData(ref fileCursor, ref currentFile);
                if(blockData == null)
                {
                    // null means end of file
                    return;
                }
                Block completedBlock = parseBlockDataIntoClass(blockData);
                
            }
        }

        private static Block parseBlockDataIntoClass(byte[] blockByteArray)
        {
            uint cursor = 0;
            Block block = new Block();

            block.versionNumber = parseFourBytesToElement(ref cursor, ref blockByteArray);
            block.previousBlockHash = parseThirtyTwoBytesToElement(ref cursor, ref blockByteArray);
            block.merkleRootHash = parseThirtyTwoBytesToElement(ref cursor, ref blockByteArray);
            block.timeStamp = parseFourBytesToElement(ref cursor, ref blockByteArray);
            block.targetDifficulty = parseFourBytesToElement(ref cursor, ref blockByteArray);
            block.nonce = parseFourBytesToElement(ref cursor, ref blockByteArray);
            block.VL_transactionCount = parseVaribleLengthInteger(ref cursor, ref blockByteArray);
            //Block class is done, moving to transaction

            for (ulong i = 0; i < block.VL_transactionCount; i++)
            {
                Transaction transaction = new Transaction();
                transaction.transactionVersionNumber = parseFourBytesToElement(ref cursor, ref blockByteArray);
                transaction.VL_inputCount = parseVaribleLengthInteger(ref cursor, ref blockByteArray);

                //fill transaction inputs
                for (ulong j = 0; j < transaction.VL_inputCount; j++)
                {
                    Input input = new Input();
                    input.transactionHash = parseThirtyTwoBytesToElement(ref cursor, ref blockByteArray);
                    input.transactionIndex = parseFourBytesToElement(ref cursor, ref blockByteArray);
                    input.VL_scriptLength = parseVaribleLengthInteger(ref cursor, ref blockByteArray);
                    input.VL_inputScript = parseCustomLengthToElement(ref cursor, ref blockByteArray, input.VL_scriptLength);
                    input.sequenceNumber = parseFourBytesToElement(ref cursor, ref blockByteArray);

                    transaction.inputs.Add(input);
                }

                transaction.VL_outputCount = parseVaribleLengthInteger(ref cursor, ref blockByteArray);

                //fill transaction outputs
                for (ulong k = 0; k < transaction.VL_outputCount; k++)
                {
                    Output output = new Output();
                    output.value = parseEightBytesToElement(ref cursor, ref blockByteArray);

                    output.VL_outputScriptLength = parseVaribleLengthInteger(ref cursor, ref blockByteArray);
                    output.VL_publicKeyAddress = parseCustomLengthToElement(ref cursor, ref blockByteArray, output.VL_outputScriptLength);

                    transaction.outputs.Add(output);
                }

                transaction.transactionLockTime = parseFourBytesToElement(ref cursor, ref blockByteArray);

                block.transactions.Add(transaction);
            }
            return block;
        }
        private static uint parseFourBytesToElement(ref uint cursor, ref byte[] blockByteArray)
        {
            byte[] buffer = new byte[4];
            Array.Copy(blockByteArray, cursor, buffer, 0, 4);
            cursor += 4;
            return BitConverter.ToUInt32(buffer, 0);
        }
        private static ulong parseEightBytesToElement(ref uint cursor, ref byte[] blockByteArray)
        {
            byte[] buffer = new byte[8];
            Array.Copy(blockByteArray, cursor, buffer, 0, 8);
            cursor += 8;
            return BitConverter.ToUInt64(buffer, 0);
        }
        private static byte[] parseThirtyTwoBytesToElement(ref uint cursor, ref byte[] blockByteArray)
        {
            byte[] buffer = new byte[32];
            Array.Copy(blockByteArray, cursor, buffer, 0, 32);
            //Array.Reverse(buffer);
            cursor += 32;
            return buffer;
        }
        private static UInt64 parseVaribleLengthInteger(ref uint cursor, ref byte[] blockByteArray)
        {
            byte[] buffer = new byte[2];
            buffer[0] = blockByteArray[cursor];
            uint transactionCount = BitConverter.ToUInt16(buffer, 0);
            if (transactionCount < 253)
            {
                cursor += 1;
                return transactionCount;
                 
            }
            else if (transactionCount == 253)
            {
                buffer = new byte[2];
                Array.Copy(blockByteArray, cursor + 1, buffer, 0, 2);
                cursor += 3;
                return BitConverter.ToUInt16(buffer, 0);
                
            }
            else if (transactionCount == 254)
            {
                buffer = new byte[4];
                Array.Copy(blockByteArray, cursor + 1, buffer, 0, 4);
                cursor += 5;
                return BitConverter.ToUInt32(buffer, 0);
            }
            else if (transactionCount == 255)
            {
                buffer = new byte[8];
                Array.Copy(blockByteArray, cursor + 1, buffer, 0, 8);
                cursor += 9;
                return BitConverter.ToUInt64(buffer, 0);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        private static byte[] parseCustomLengthToElement(ref uint cursor, ref byte[] blockByteArray, ulong length)
        {
            byte[] buffer = new byte[length];
            Array.Copy(blockByteArray, cursor, buffer, 0, (uint)length);
            //Array.Reverse(buffer);
            cursor += (uint)length;
            return buffer;
        }
    }
}