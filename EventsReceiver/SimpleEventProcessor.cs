﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace EventsReceiver
{
    public class SimpleEventProcessor : IEventProcessor
    {
        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
            return Task.CompletedTask;
        }


        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                if (data.Contains("MonthlyMembershipFee"))
                {
                    continue;
                }

                using (var file = File.AppendText("./events.log"))
                    file.WriteLine(data);
            }

            return context.CheckpointAsync();
        }
    }
}
