using DiegoG.Utilities.IO.Files;
using System.Net;
using System.Text;

var memstream = new MemoryStream();
var memstreamsend = new MemoryStream(Encoding.UTF8.GetBytes("There are six hundred twenty seven thousand trillion stars in the universe, and likely much more"));
var receiver = new FileReceiverTask(IPAddress.Loopback, 65321, memstream, 1024 * 1024);
var transmitter = new FileTransmitterTask(IPAddress.Loopback, 65321, memstreamsend, 1024 * 1024);

await receiver.WaitForConnection();
await transmitter.Transmit();
await receiver.Receive();

var result = Encoding.UTF8.GetString(memstreamsend.ToArray());
;