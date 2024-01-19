using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Connection
{
    public int sourceId;
    public byte sourceBit;
    public int destinationId;
    public byte destinationBit;
    public float weight;
    public Connection(byte sourceBit, int sourceId, byte destinationBit, int destinationId, float weight)
    {
        this.sourceBit = sourceBit;
        this.sourceId = sourceId;
        this.destinationBit = destinationBit;
        this.destinationId = destinationId;
        this.weight = weight;
    }
}

