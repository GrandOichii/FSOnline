using System.Text;
using System.Net.Sockets;

namespace FSCore.Utility;

static public class NetUtility
{
    /// <summary>
    /// Reads a string from an output stream
    /// </summary>
    /// <param name="stream">Output string</param>
    /// <returns>The read message</returns>
    static public string Read(NetworkStream stream) {
        byte[] lengthBuffer = new byte[4];
        int bytesRead = stream.Read(lengthBuffer, 0, lengthBuffer.Length);
        if (bytesRead == 0) return ""; // Client disconnected
        int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
        byte[] buffer = new byte[messageLength];
        bytesRead = stream.Read(buffer, 0, buffer.Length);
        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        return message;
    }    

    /// <summary>
    /// Writes a message to the input stream
    /// </summary>
    /// <param name="stream">Input stream</param>
    /// <param name="message">Message to be written</param>
    static public void Write(NetworkStream stream, string message) {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        byte[] lengthBuffer = BitConverter.GetBytes(buffer.Length);
        stream.Write(lengthBuffer, 0, lengthBuffer.Length);
        stream.Write(buffer, 0, buffer.Length);
    }
}