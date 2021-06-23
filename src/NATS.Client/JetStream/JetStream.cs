﻿// Copyright 2021 The NATS Authors
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Text;
using System.Threading.Tasks;
using NATS.Client.Internals;

namespace NATS.Client.JetStream
{
    public class JetStream : IJetStream, IJetStreamManagement
    {
        public string Prefix { get; }
        public JetStreamOptions Options { get; }
        public IConnection Connection { get; }
        public int Timeout { get; }

        private static readonly PublishOptions defaultPubOpts = PublishOptions.Builder().Build();

        internal JetStream(IConnection connection, JetStreamOptions options)
        {
            Connection = connection;
            Options = options ?? JetStreamOptions.Builder().Build();
            Prefix = Options.Prefix;
            Timeout = (int)Options.RequestTimeout.Millis;

            CheckJetStream();
        }

        private string AddPrefix(string subject) => Prefix + subject;

        internal Msg JSRequest(string subject, byte[] bytes, int timeout)
        {
            return Connection.Request(AddPrefix(subject), bytes, timeout);
        }

        public void CheckJetStream()
        {
            try
            {
                Msg m = JSRequest(JetStreamConstants.JsapiAccountInfo, null, Timeout);
                var s = new AccountStatistics(m);
                if (s.ErrorCode == 503)
                {
                    throw new NATSJetStreamException(s.ErrorDescription);
                }
            }
            catch (NATSNoRespondersException nre)
            {
                throw new NATSJetStreamException("JetStream is not available.", nre);
            }
            catch (NATSTimeoutException te)
            {
                throw new NATSJetStreamException("JetStream did not respond.", te);
            }
            catch (Exception e)
            {
                throw new NATSJetStreamException("An exception occurred communicating with JetStream.", e);
            }
        }

        // Build the headers.  Take care not to unnecessarily allocate, we're
        // in the fastpath here.
        private MsgHeader MergeHeaders(MsgHeader headers, PublishOptions opts)
        {
            if (opts == null)
                return null;

            MsgHeader mh = null;

            if (!string.IsNullOrEmpty(opts.ExpectedLastMsgId))
            {
                mh = headers ?? new MsgHeader();
                mh.Set(MsgHeader.ExpLastIdHeader, opts.ExpectedLastMsgId);
            }
            if (opts.ExpectedLastSeq >= 0)
            {
                mh = headers ?? new MsgHeader();
                mh.Set(MsgHeader.ExpLastSeqHeader, opts.ExpectedLastSeq.ToString());
            }
            if (!string.IsNullOrEmpty(opts.ExpectedStream))
            {
                mh = headers ?? new MsgHeader();
                mh.Set(MsgHeader.ExpStreamHeader, opts.ExpectedStream);
            }
            if (!string.IsNullOrEmpty(opts.MessageId))
            {
                mh = headers ?? new MsgHeader();
                mh.Set(MsgHeader.MsgIdHeader, opts.MessageId);
            }

            return mh;
        }

        private PublishAck PublishSync(string subject, byte[] data, PublishOptions opts)
            => PublishSync(new Msg(subject, null, data), opts);


        private PublishAck PublishSync(Msg msg, PublishOptions opts)
        {
            if (msg.HasHeaders)
            {
                MergeHeaders(msg.header, opts);
            }
            else
            {
                msg.header = MergeHeaders(null, opts);
            }

            return new PublishAck(Connection.Request(msg));
        }

        public PublishAck Publish(string subject, byte[] data) => PublishSync(subject, data, null);

        public PublishAck Publish(string subject, byte[] data, PublishOptions options) => PublishSync(subject, data, options);

        public PublishAck Publish(Msg message)
        {
            return PublishSync(message, defaultPubOpts);
        }

        public PublishAck Publish(Msg message, PublishOptions publishOptions)
        {
            throw new NotImplementedException();
        }

        public Task<PublishAck> PublishAsync(string subject, byte[] data)
        {
            throw new NotImplementedException();
        }

        public Task<PublishAck> PublishAsync(string subject, byte[] data, PublishOptions publishOptions)
        {
            throw new NotImplementedException();
        }

        public Task<PublishAck> PublishAsync(Msg message)
        {
            throw new NotImplementedException();
        }

        public Task<PublishAck> PublishAsync(Msg message, PublishOptions publishOptions)
        {
            throw new NotImplementedException();
        }

        public IJetStreamPullSubscription PullSubscribe(string subject, SubscribeOptions options)
        {
            throw new NotImplementedException();
        }

        public IJetStreamPushSubscription Subscribe(string subject, EventHandler<MsgHandlerEventArgs> handler, PullSubscribeOptions options)
        {
            throw new NotImplementedException();
        }

        public IJetStreamSyncSubscription SyncSubsribe(string subject, SubscribeOptions options)
        {
            throw new NotImplementedException();
        }

        public IJetStreamPullSubscription PullSubscribe(string subject)
        {
            throw new NotImplementedException();
        }

        public IJetStreamPushSubscription Subscribe(string subject, EventHandler<MsgHandlerEventArgs> handler)
        {
            throw new NotImplementedException();
        }

        public IJetStreamPushSubscription Subscribe(string subject, string queue, EventHandler<MsgHandlerEventArgs> handler)
        {
            throw new NotImplementedException();
        }

        public IJetStreamPushSubscription Subscribe(string subject, string queue, EventHandler<MsgHandlerEventArgs> handler, PullSubscribeOptions options)
        {
            throw new NotImplementedException();
        }

        public IJetStreamSyncSubscription SyncSubsribe(string subject)
        {
            throw new NotImplementedException();
        }

        public IJetStreamSyncSubscription SyncSubsribe(string subject, string queue)
        {
            throw new NotImplementedException();
        }

        public IJetStreamSyncSubscription SyncSubsribe(string subject, string queue, SubscribeOptions options)
        {
            throw new NotImplementedException();
        }

        public IJetStreamPullSubscription SubscribePull(string subject)
        {
            throw new NotImplementedException();
        }

        public IJetStreamPullSubscription SubscribePull(string subject, PullSubscribeOptions options)
        {
            throw new NotImplementedException();
        }

        public IJetStreamPushSubscription Subscribe(string subject, EventHandler<MsgHandlerEventArgs> handler, PushSubscribeOptions options)
        {
            throw new NotImplementedException();
        }

        public IJetStreamPushSubscription Subscribe(string subject, string queue, EventHandler<MsgHandlerEventArgs> handler, PushSubscribeOptions options)
        {
            throw new NotImplementedException();
        }

        public IJetStreamSyncSubscription SubscribeSync(string subject)
        {
            throw new NotImplementedException();
        }

        public IJetStreamSyncSubscription SubscribeSync(string subject, SubscribeOptions options)
        {
            throw new NotImplementedException();
        }

        public IJetStreamSyncSubscription SubscribeSync(string subject, string queue)
        {
            throw new NotImplementedException();
        }

        public IJetStreamSyncSubscription SubscribeSync(string subject, string queue, SubscribeOptions options)
        {
            throw new NotImplementedException();
        }


        #region JetStreamManagement

        public StreamInfo AddStream(StreamConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var m = JSRequest(
                string.Format(JetStreamConstants.JsapiStreamCreate, config.Name),
                config.Serialize(), Timeout);

            return new StreamInfo(new ApiResponse(m, true).JsonNode);
        }

        public StreamInfo UpdateStream(StreamConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var m = JSRequest(
                string.Format(JetStreamConstants.JsapiStreamUpdate, config.Name),
                config.Serialize(),
                Timeout);

            return new StreamInfo(new ApiResponse(m, true).JsonNode);
        }

        public bool DeleteStream(string streamName)
        {
            if (streamName == null)
            {
                throw new ArgumentNullException(nameof(streamName));
            }

            var m = JSRequest(
                string.Format(JetStreamConstants.JsapiStreamDelete, streamName),
                null,
                Timeout);

            return new SuccessApiResponse(Encoding.ASCII.GetString(m.Data)).Success;
        }

        public StreamInfo GetStreamInfo(string streamName)
        {
            if (streamName == null)
            {
                throw new ArgumentNullException(nameof(streamName));
            }

            var m = JSRequest(
                string.Format(JetStreamConstants.JsapiStreamInfo, streamName),
                null,
                Timeout);

            return new StreamInfo(new ApiResponse(m, true).JsonNode);
        }

        public PurgeResponse PurgeStream(string streamName)
        {
            if (streamName == null)
            {
                throw new ArgumentNullException(nameof(streamName));
            }

            var m = JSRequest(
                string.Format(JetStreamConstants.JsapiStreamPurge, streamName),
                null,
                Timeout);

            return new PurgeResponse(new ApiResponse(m, true).JsonNode);
        }

        public ConsumerInfo AddConsumer(string streamName, ConsumerConfiguration config)
        {
            if (streamName == null)
            {
                // TODO validate stream name
                throw new ArgumentNullException(nameof(streamName));
            }
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            string subj;

            if (config.Durable == null)
            {
                subj = string.Format(JetStreamConstants.JsapiConsumerCreate, streamName);
            }
            else
            {
                subj = string.Format(JetStreamConstants.JsapiDurableCreate, streamName,
                    config.Durable);
            }

            var ccr = new ConsumerCreateRequest(streamName, config);
            var m = JSRequest(subj, ccr.Serialize(), Timeout);
            return new ConsumerInfo(new ApiResponse(m, true).JsonNode);
        }

        public bool DeleteConsumer(string streamName, string consumer)
        {
            if (streamName == null)
            {
                // TODO validate stream name
                throw new ArgumentNullException(nameof(streamName));
            }
            if (consumer == null)
            {
                throw new ArgumentNullException(nameof(consumer));
            }

            var m = JSRequest(
                string.Format(JetStreamConstants.JsapiConsumerDelete, streamName, consumer),
                null,
                Timeout);

            return new SuccessApiResponse(Encoding.ASCII.GetString(m.Data)).Success;
        }

        public ConsumerInfo GetConsumerInfo(string streamName, string consumer)
        {
            if (streamName == null)
            {
                // TODO validate stream name
                throw new ArgumentNullException(nameof(streamName));
            }
            if (consumer == null)
            {
                throw new ArgumentNullException(nameof(consumer));
            }

            var m = JSRequest(
                string.Format(JetStreamConstants.JsapiConsumerInfo, streamName, consumer),
                null,
                Timeout);

            return new ConsumerInfo(new ApiResponse(m, true).JsonNode);
        }

        public string[] GetConsumerNames(string streamName)
        {
            throw new NotImplementedException();
        }

        public ConsumerInfo[] GetConsumers(string streamName)
        {
            throw new NotImplementedException();
        }

        public string[] GetStreamNames()
        {
            throw new NotImplementedException();
        }

        public StreamInfo[] GetStreams()
        {
            throw new NotImplementedException();
        }

        public MessageInfo GetMessage(string streamName, long sequence)
        {
            throw new NotImplementedException();
        }

        public bool DeleteMessage(string streamName, long sequence)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
