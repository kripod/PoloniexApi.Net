using System;

namespace Jojatekok.PoloniexAPI
{
    public class TrollboxMessageEventArgs : EventArgs
    {
        public string SenderName { get; private set; }
        public uint SenderReputation { get; private set; }
        public string MessageType { get; private set; }
        public ulong MessageNumber { get; private set; }
        public string MessageText { get; private set; }

        internal TrollboxMessageEventArgs(string senderName, uint senderReputation, string messageType, ulong messageNumber, string messageText)
        {
            SenderName = senderName;
            SenderReputation = senderReputation;
            MessageType = messageType;
            MessageNumber = messageNumber;
            MessageText = messageText;
        }
    }
}
