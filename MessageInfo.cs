using System;

// https://stackoverflow.com/a/17547737/1389817
namespace mvp
{
    public class MessageInfo {
        public DateTime DateCreated = DateTime.UtcNow;
        public Guid CorrelationId = Guid.NewGuid();
    }
}
