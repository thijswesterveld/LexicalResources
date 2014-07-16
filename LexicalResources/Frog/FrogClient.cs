using NLP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Frog
{
    public class FrogClient
    {
        private static Socket frogServer;
        
        public static void StartClient()
        {
            // Connect to a remote device.
            try
            {
                /// Establish the remote endpoint for the socket.
                /// We use port 4423 on the local computer.
                /// Frog needs to be started with: frog --skip=mpnca -S 4423
                /// The skip options tell frog to skip certain slow processing steps we don't need 
                /// multi-word unit chunking for the parser (m)
                /// parsing (p)
                /// named-entity recognition (n)
                /// base phrase chunking (c)
                /// Morphological Analyzer (a)
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                AddressFamily addressFamily = ipHostInfo.AddressList[0].AddressFamily;
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 4423);

                // Create a TCP/IP  socket.
                frogServer = new Socket(addressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    frogServer.Connect(remoteEP);
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void StopClient()
        {
            // Release the socket.
            frogServer.Shutdown(SocketShutdown.Both);
            frogServer.Close();
        }

        public static List<Token> Analyse(string text)
        {
            List<Token> result = new List<Token>();
            try
            {
                if (frogServer == null || !frogServer.Connected)
                {
                    StartClient();
                }
                SendText(text);

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

                sw.Start();
                MemoryStream frogResponse = ReceiveResponse();
                sw.Stop();
                Console.WriteLine("Receive: {0}ms", sw.ElapsedMilliseconds);

                sw.Restart();
                List<FrogToken> frogOutput = parseResponse(frogResponse);
                sw.Stop();
                Console.WriteLine("Parse: {0}ms", sw.ElapsedMilliseconds);

                sw.Restart();
                result = AddOffsets(frogOutput, text);
                sw.Stop();
                Console.WriteLine("AddOffsets: {0}ms", sw.ElapsedMilliseconds);
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
            return result;
        }

        private static void SendText(string text)
        {
            // Encode the data string into a byte array.
            byte[] msg = Encoding.UTF8.GetBytes(text + "\r\nEOT\r\n");

            // Send the data through the socket.
            int bytesSent = frogServer.Send(msg);

        }

        static byte[] readyMsg = Encoding.UTF8.GetBytes("\nREADY\n");

        private static MemoryStream ReceiveResponse()
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[1024];

            byte[] lastBytes = new byte[7];

            MemoryStream response = new MemoryStream();
            bool responseComplete = false;
            while (!responseComplete)
            {
                // Receive the response from the remote device.
                int bytesRec = frogServer.Receive(bytes);
                
                // Append to response stream
                response.Write(bytes,0,bytesRec);
                
                /// Check if responseComplete: message ends with \nREADY\n
                /// This message may be broken over multiple packages, if current bytesRec smaller than 
                /// the size of \nREADY\n (7), we need to combine with previously received data.
                /// E.g. last bytes from precious receive   [.....\nR] (7 bytes)
                /// new response:           [EADY\n] (5 bytes)
                
                /// Normally, more than 7 bytes are available in the current response
                /// -> nothing from the previous buffer to keep, last 7 of new buffer to copy
                int oldBytesToKeep = 0;
                int newBytesToAppend = 7;
                if(bytesRec < 7)
                {
                    // new buffer is too short, keep additional bytes from prev buffer
                    oldBytesToKeep = 7 - bytesRec;
                    newBytesToAppend = bytesRec;

                    /// Copy last bytes from precious receive [.....\nR] (7 bytes)
                    /// to beginning of buffer:               [\nR.....] (7 bytes)
                    Buffer.BlockCopy(lastBytes, newBytesToAppend, lastBytes, 0, oldBytesToKeep);
                }
                /// Append new bytes
                Buffer.BlockCopy(bytes, bytesRec - newBytesToAppend, lastBytes, oldBytesToKeep, newBytesToAppend);

                /// check if lastbytes equal responseComplete message
                responseComplete = lastBytes.SequenceEqual(readyMsg);
            }
            response.Position = 0;
            return response;
        }

        private static List<FrogToken> parseResponse(MemoryStream frogResponse)
        {
            List<FrogToken> result = new List<FrogToken>();
            TextReader tr = new StreamReader(frogResponse, Encoding.UTF8);
            string line = tr.ReadLine();
            while(line != null)
            {
                FrogToken token = new FrogToken(line);
                if (token.TokenNumber > 0)
                {
                    /// Add only if this is a token (and not an empty line or something else)
                    result.Add(token);
                }
                line = tr.ReadLine();
            }
            return result;
        }

        private static List<Token> AddOffsets(List<FrogToken> frogOutput, string text)
        {
            List<Token> result = new List<Token>();
            int textPointer = 0;
            foreach(FrogToken ft in frogOutput)
            {
                int startOffset = text.IndexOf(ft.Term,textPointer);
                int endOffset = startOffset + ft.Term.Length;
                textPointer = endOffset;

                result.Add(new Token()
                {
                    POS=ft.POS,
                    Term =ft.Term,
                    Stem = ft.Lemma,
                    StartOffset = startOffset,
                    EndOffset = endOffset
                });
            }
            return result;
        }

    }
}
