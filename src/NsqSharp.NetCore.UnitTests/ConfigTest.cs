using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Authentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NsqSharp.Core;
using NsqSharp.NetCore.UnitTestMethods;
using NsqSharp.Utils;
using NsqSharp.Utils.Attributes;
using NsqSharp.Utils.Extensions;

namespace NsqSharp.Tests
{
    [TestClass]
    public class ConfigTest
    {
        [TestMethod]
        public void TestConfigSet()
        {
            var c = new NsqConfig();

            AssertHelper.Throws<Exception>(() => c.Set("not a real config value", new object()),
                "No error when setting an invalid value");
        }

        [TestMethod]
        public void TestConfigValidateDefault()
        {
            var c = new NsqConfig();
            c.Validate();
        }

        [TestMethod]
        public void TestConfigValidateError()
        {
            var c = new NsqConfig();

            AssertHelper.Throws<Exception>(() => c.Set("max_backoff_duration", "24h"));

            // property wasn't set, state should still be ok
            c.Validate();

            // test MaxBackoffDuration validation
            c.MaxBackoffDuration = TimeSpan.FromHours(24);
            Assert.AreEqual(TimeSpan.FromHours(24), c.MaxBackoffDuration);
            AssertHelper.Throws<Exception>(c.Validate);

            // reset to good state
            c.MaxBackoffDuration = TimeSpan.FromSeconds(60);
            c.Validate();

            // make sure another bad value and call to Validate still throws after the good Validate
            c.MaxBackoffDuration = TimeSpan.FromHours(24);
            Assert.AreEqual(TimeSpan.FromHours(24), c.MaxBackoffDuration);
            AssertHelper.Throws<Exception>(c.Validate);

            // reset to good state
            c.MaxBackoffDuration = TimeSpan.FromSeconds(60);
            c.Validate();

            // test null BackoffStrategy validation
            c.BackoffStrategy = null;
            Assert.IsNull(c.BackoffStrategy);
            AssertHelper.Throws<Exception>(c.Validate);
        }

        [TestMethod]
        public void TestOptNamesUnique()
        {
            var list = new List<string>();
            foreach (var propertyInfo in typeof(NsqConfig).GetProperties())
            {
                var opt = propertyInfo.Get<OptAttribute>();

                string option = opt.Name;

                Assert.IsFalse(list.Contains(option),
                    string.Format("property opt '{0}' exists more than once", option));

                list.Add(option);
            }
        }

        [TestMethod]
        public void TestOptNamesAreLowerAndTrimmed()
        {
            foreach (var propertyInfo in typeof(NsqConfig).GetProperties())
            {
                var opt = propertyInfo.Get<OptAttribute>();

                bool hasGoodName = opt.Name == opt.Name.ToLower().Trim() && !opt.Name.Contains("-");
                Assert.IsTrue(hasGoodName, string.Format("property opt '{0}' does not match naming rules", opt.Name));
            }
        }

        [TestMethod]
        public void TestDefaultValues()
        {
            var c = new NsqConfig();

            Assert.AreEqual(TimeSpan.FromSeconds(1), c.DialTimeout, "dial_timeout");
            Assert.AreEqual(TimeSpan.FromSeconds(60), c.ReadTimeout, "read_timeout");
            Assert.AreEqual(TimeSpan.FromSeconds(10), c.WriteTimeout, "write_timeout");
            Assert.AreEqual(TimeSpan.FromSeconds(60), c.LookupdPollInterval, "lookupd_poll_interval");
            Assert.AreEqual(0.3, c.LookupdPollJitter, "lookupd_poll_jitter");
            Assert.AreEqual(TimeSpan.FromMinutes(15), c.MaxRequeueDelay, "max_requeue_delay");
            Assert.AreEqual(TimeSpan.FromSeconds(90), c.DefaultRequeueDelay, "default_requeue_delay");
            Assert.AreEqual(typeof(ExponentialStrategy), c.BackoffStrategy.GetType(), "backoff_strategy");
            Assert.AreEqual(TimeSpan.FromMinutes(2), c.MaxBackoffDuration, "max_backoff_duration");
            Assert.AreEqual(TimeSpan.FromSeconds(1), c.BackoffMultiplier, "backoff_multiplier");
            Assert.AreEqual(5, c.MaxAttempts, "max_attempts");
            Assert.AreEqual(TimeSpan.FromSeconds(10), c.LowRdyIdleTimeout, "low_rdy_idle_timeout");
            Assert.AreEqual(TimeSpan.FromSeconds(5), c.RDYRedistributeInterval, "rdy_redistribute_interval");
            Assert.AreEqual(false, c.RDYRedistributeOnIdle, "rdy_redistribute_on_idle");
            Assert.AreEqual(OS.Hostname().Split('.')[0], c.ClientID, "client_id");
            Assert.AreEqual(OS.Hostname(), c.Hostname, "hostname");
            Assert.AreEqual(string.Format("{0}/{1}", ClientInfo.ClientName, ClientInfo.Version), c.UserAgent,
                "user_agent");
            Assert.AreEqual(TimeSpan.FromSeconds(30), c.HeartbeatInterval, "heartbeat_interval");
            Assert.AreEqual(0, c.SampleRate, "sample_rate");
            Assert.IsNull(c.TlsConfig, "tls_config");
            //Assert.AreEqual(false, c.Deflate, "deflate"); // TODO: Deflate
            //Assert.AreEqual(6, c.DeflateLevel, "deflate_level"); // TODO: Deflate
            //Assert.AreEqual(false, c.Snappy, "snappy"); // TODO: Snappy
            Assert.AreEqual(16384, c.OutputBufferSize, "output_buffer_size");
            Assert.AreEqual(TimeSpan.FromMilliseconds(250), c.OutputBufferTimeout, "output_buffer_timeout");
            Assert.AreEqual(1, c.MaxInFlight, "max_in_flight");
            Assert.AreEqual(TimeSpan.Zero, c.MessageTimeout, "msg_timeout");
            Assert.IsNull(c.AuthSecret, "auth_secret");
        }

        [TestMethod]
        public void TestMinValues()
        {
            var c = new NsqConfig();
            c.Set("read_timeout", TimeSpan.FromMilliseconds(100));
            c.Set("write_timeout", TimeSpan.FromMilliseconds(100));
            c.Set("lookupd_poll_interval", TimeSpan.FromMilliseconds(10));
            c.Set("lookupd_poll_jitter", 0);
            c.Set("max_requeue_delay", TimeSpan.Zero);
            c.Set("default_requeue_delay", TimeSpan.Zero);
            c.Set("backoff_strategy", "exponential");
            c.Set("max_backoff_duration", TimeSpan.Zero);
            c.Set("backoff_multiplier", "1ms");
            c.Set("max_attempts", 0);
            c.Set("low_rdy_idle_timeout", TimeSpan.FromSeconds(1));
            c.Set("rdy_redistribute_interval", TimeSpan.FromMilliseconds(1));
            c.Set("client_id", null);
            c.Set("hostname", null);
            c.Set("user_agent", null);
            c.Set("heartbeat_interval", TimeSpan.MinValue);
            c.Set("sample_rate", 0);
            c.Set("tls_config", null);
            //c.Set("deflate", false); // TODO: Deflate
            //c.Set("deflate_level", 1); // TODO: Deflate
            //c.Set("snappy", false); // TODO: Snappy
            c.Set("output_buffer_size", long.MinValue);
            c.Set("output_buffer_timeout", TimeSpan.MinValue);
            c.Set("max_in_flight", 0);
            c.Set("msg_timeout", TimeSpan.Zero);
            c.Set("auth_secret", null);

            Assert.AreEqual(TimeSpan.FromMilliseconds(100), c.ReadTimeout, "read_timeout");
            Assert.AreEqual(TimeSpan.FromMilliseconds(100), c.WriteTimeout, "write_timeout");
            Assert.AreEqual(TimeSpan.FromMilliseconds(10), c.LookupdPollInterval, "lookupd_poll_interval");
            Assert.AreEqual(0, c.LookupdPollJitter, "lookupd_poll_jitter");
            Assert.AreEqual(TimeSpan.Zero, c.MaxRequeueDelay, "max_requeue_delay");
            Assert.AreEqual(TimeSpan.Zero, c.DefaultRequeueDelay, "default_requeue_delay");
            Assert.AreEqual(typeof(ExponentialStrategy), c.BackoffStrategy.GetType(), "backoff_strategy");
            Assert.AreEqual(TimeSpan.Zero, c.MaxBackoffDuration, "max_backoff_duration");
            Assert.AreEqual(TimeSpan.FromMilliseconds(1), c.BackoffMultiplier, "backoff_multiplier");
            Assert.AreEqual(0, c.MaxAttempts, "max_attempts");
            Assert.AreEqual(TimeSpan.FromSeconds(1), c.LowRdyIdleTimeout, "low_rdy_idle_timeout");
            Assert.AreEqual(TimeSpan.FromMilliseconds(1), c.RDYRedistributeInterval, "rdy_redistribute_interval");
            Assert.AreEqual(null, c.ClientID, "client_id");
            Assert.AreEqual(null, c.Hostname, "hostname");
            Assert.AreEqual(null, c.UserAgent, "user_agent");
            Assert.AreEqual(TimeSpan.MinValue, c.HeartbeatInterval, "heartbeat_interval");
            Assert.AreEqual(0, c.SampleRate, "sample_rate");
            Assert.AreEqual(null, c.TlsConfig, "tls_config");
            //Assert.AreEqual(false, c.Deflate, "deflate"); // TODO: Deflate
            //Assert.AreEqual(1, c.DeflateLevel, "deflate_level"); // TODO: Deflate
            //Assert.AreEqual(false, c.Snappy, "snappy"); // TODO: Snappy
            Assert.AreEqual(long.MinValue, c.OutputBufferSize, "output_buffer_size");
            Assert.AreEqual(TimeSpan.MinValue, c.OutputBufferTimeout, "output_buffer_timeout");
            Assert.AreEqual(0, c.MaxInFlight, "max_in_flight");
            Assert.AreEqual(TimeSpan.Zero, c.MessageTimeout, "msg_timeout");
            Assert.AreEqual(null, c.AuthSecret, "auth_secret");
        }

        [TestMethod]
        public void TestMaxValues()
        {
            var c = new NsqConfig();
            var tlsConfig = new TlsConfig();
            c.Set("read_timeout", TimeSpan.FromMinutes(5));
            c.Set("write_timeout", TimeSpan.FromMinutes(5));
            c.Set("lookupd_poll_interval", TimeSpan.FromMinutes(5));
            c.Set("lookupd_poll_jitter", 1);
            c.Set("max_requeue_delay", TimeSpan.FromMinutes(60));
            c.Set("default_requeue_delay", TimeSpan.FromMinutes(60));
            c.Set("backoff_strategy", "full_jitter");
            c.Set("max_backoff_duration", TimeSpan.FromMinutes(60));
            c.Set("backoff_multiplier", TimeSpan.FromMinutes(60));
            c.Set("max_attempts", 65535);
            c.Set("low_rdy_idle_timeout", TimeSpan.FromMinutes(5));
            c.Set("rdy_redistribute_interval", TimeSpan.FromSeconds(5));
            c.Set("client_id", "my");
            c.Set("hostname", "my.host.name.com");
            c.Set("user_agent", "user-agent/1.0");
            c.Set("heartbeat_interval", TimeSpan.MaxValue);
            c.Set("sample_rate", 99);
            c.Set("tls_config", tlsConfig);
            //c.Set("deflate", true); // TODO: Deflate
            //c.Set("deflate_level", 9); // TODO: Deflate
            //c.Set("snappy", true); // TODO: Snappy
            c.Set("output_buffer_size", long.MaxValue);
            c.Set("output_buffer_timeout", TimeSpan.MaxValue);
            c.Set("max_in_flight", int.MaxValue);
            c.Set("msg_timeout", TimeSpan.MaxValue);
            c.Set("auth_secret", "!@#@#$#%");

            Assert.AreEqual(TimeSpan.FromMinutes(5), c.ReadTimeout, "read_timeout");
            Assert.AreEqual(TimeSpan.FromMinutes(5), c.WriteTimeout, "write_timeout");
            Assert.AreEqual(TimeSpan.FromMinutes(5), c.LookupdPollInterval, "lookupd_poll_interval");
            Assert.AreEqual(1, c.LookupdPollJitter, "lookupd_poll_jitter");
            Assert.AreEqual(TimeSpan.FromMinutes(60), c.MaxRequeueDelay, "max_requeue_delay");
            Assert.AreEqual(TimeSpan.FromMinutes(60), c.DefaultRequeueDelay, "default_requeue_delay");
            Assert.AreEqual(typeof(FullJitterStrategy), c.BackoffStrategy.GetType(), "backoff_strategy");
            Assert.AreEqual(TimeSpan.FromMinutes(60), c.MaxBackoffDuration, "max_backoff_duration");
            Assert.AreEqual(TimeSpan.FromMinutes(60), c.BackoffMultiplier, "backoff_multiplier");
            Assert.AreEqual(65535, c.MaxAttempts, "max_attempts");
            Assert.AreEqual(TimeSpan.FromMinutes(5), c.LowRdyIdleTimeout, "low_rdy_idle_timeout");
            Assert.AreEqual(TimeSpan.FromSeconds(5), c.RDYRedistributeInterval, "rdy_redistribute_interval");
            Assert.AreEqual("my", c.ClientID, "client_id");
            Assert.AreEqual("my.host.name.com", c.Hostname, "hostname");
            Assert.AreEqual("user-agent/1.0", c.UserAgent, "user_agent");
            Assert.AreEqual(TimeSpan.MaxValue, c.HeartbeatInterval, "heartbeat_interval");
            Assert.AreEqual(99, c.SampleRate, "sample_rate");
            Assert.AreEqual(tlsConfig, c.TlsConfig, "tls_config");
            //Assert.AreEqual(true, c.Deflate, "deflate"); // TODO: Deflate
            //Assert.AreEqual(9, c.DeflateLevel, "deflate_level"); // TODO: Deflate
            //Assert.AreEqual(true, c.Snappy, "snappy"); // TODO: Snappy
            Assert.AreEqual(long.MaxValue, c.OutputBufferSize, "output_buffer_size");
            Assert.AreEqual(TimeSpan.MaxValue, c.OutputBufferTimeout, "output_buffer_timeout");
            Assert.AreEqual(int.MaxValue, c.MaxInFlight, "max_in_flight");
            Assert.AreEqual(TimeSpan.MaxValue, c.MessageTimeout, "msg_timeout");
            Assert.AreEqual("!@#@#$#%", c.AuthSecret, "auth_secret");
        }

        [TestMethod]
        public void TestValidatesLessThanMinValues()
        {
            var c = new NsqConfig();
            var tick = new TimeSpan(1);

            AssertHelper.Throws<Exception>(() => c.Set("read_timeout", TimeSpan.FromMilliseconds(100) - tick),
                "read_timeout");
            AssertHelper.Throws<Exception>(() => c.Set("write_timeout", TimeSpan.FromMilliseconds(100) - tick),
                "write_timeout");
            AssertHelper.Throws<Exception>(() => c.Set("lookupd_poll_interval", TimeSpan.FromMilliseconds(10) - tick),
                "lookupd_poll_interval");
            AssertHelper.Throws<Exception>(() => c.Set("lookupd_poll_jitter", 0 - double.Epsilon), "lookupd_poll_jitter");
            AssertHelper.Throws<Exception>(() => c.Set("max_requeue_delay", TimeSpan.Zero - tick), "max_requeue_delay");
            AssertHelper.Throws<Exception>(() => c.Set("default_requeue_delay", TimeSpan.Zero - tick),
                "default_requeue_delay");
            AssertHelper.Throws<Exception>(() => c.Set("backoff_strategy", "invalid"), "backoff_strategy");
            AssertHelper.Throws<Exception>(() => c.Set("max_backoff_duration", TimeSpan.Zero - tick), "max_backoff_duration");
            AssertHelper.Throws<Exception>(() => c.Set("backoff_multiplier", TimeSpan.FromMilliseconds(1) - tick),
                "backoff_multiplier");
            AssertHelper.Throws<Exception>(() => c.Set("max_attempts", 0 - 1), "max_attempts");
            AssertHelper.Throws<Exception>(() => c.Set("low_rdy_idle_timeout", TimeSpan.FromSeconds(1) - tick),
                "low_rdy_idle_timeout");
            AssertHelper.Throws<Exception>(() => c.Set("rdy_redistribute_interval", TimeSpan.FromMilliseconds(1) - tick),
                "rdy_redistribute_interval");
            //c.Set("client_id", null);
            //c.Set("hostname", null);
            //c.Set("user_agent", null);
            //AssertHelper.Throws<Exception>(() => c.Set("heartbeat_interval", TimeSpan.MinValue - tick), "heartbeat_interval");
            AssertHelper.Throws<Exception>(() => c.Set("sample_rate", 0 - 1), "sample_rate");
            //c.Set("tls_config", null);
            //c.Set("deflate", false);
            AssertHelper.Throws<Exception>(() => c.Set("deflate_level", 1 - 1), "deflate_level");
            //c.Set("snappy", false);
            //AssertHelper.Throws<Exception>(() => c.Set("output_buffer_size", Int64.MinValue - 1), "");
            //AssertHelper.Throws<Exception>(() => c.Set("output_buffer_timeout", TimeSpan.MinValue - tick), "output_buffer_timeout");
            AssertHelper.Throws<Exception>(() => c.Set("max_in_flight", 0 - 1), "max_in_flight");
            AssertHelper.Throws<Exception>(() => c.Set("msg_timeout", TimeSpan.Zero - tick), "msg_timeout");
            //AssertHelper.Throws<Exception>(() => c.Set("auth_secret", null), "");
        }

        [TestMethod]
        public void TestValidatesGreaterThanMaxValues()
        {
            var c = new NsqConfig();
            var tick = new TimeSpan(1);

            AssertHelper.Throws<Exception>(() => c.Set("read_timeout", TimeSpan.FromMinutes(5) + tick), "read_timeout");
            AssertHelper.Throws<Exception>(() => c.Set("write_timeout", TimeSpan.FromMinutes(5) + tick), "write_timeout");
            AssertHelper.Throws<Exception>(() => c.Set("lookupd_poll_interval", TimeSpan.FromMinutes(5) + tick),
                "lookupd_poll_interval");
            AssertHelper.Throws<Exception>(() => c.Set("lookupd_poll_jitter", 1 + 0.0001), "lookupd_poll_jitter");
            AssertHelper.Throws<Exception>(() => c.Set("max_requeue_delay", TimeSpan.FromMinutes(60) + tick),
                "max_requeue_delay");
            AssertHelper.Throws<Exception>(() => c.Set("default_requeue_delay", TimeSpan.FromMinutes(60) + tick),
                "default_requeue_delay");
            AssertHelper.Throws<Exception>(() => c.Set("backoff_strategy", "invalid"), "backoff_strategy");
            AssertHelper.Throws<Exception>(() => c.Set("max_backoff_duration", TimeSpan.FromMinutes(60) + tick),
                "max_backoff_duration");
            AssertHelper.Throws<Exception>(() => c.Set("backoff_multiplier", TimeSpan.FromMinutes(60) + tick),
                "backoff_multiplier");
            AssertHelper.Throws<Exception>(() => c.Set("max_attempts", 65535 + 1), "max_attempts");
            AssertHelper.Throws<Exception>(() => c.Set("low_rdy_idle_timeout", TimeSpan.FromMinutes(5) + tick),
                "low_rdy_idle_timeout");
            AssertHelper.Throws<Exception>(() => c.Set("rdy_redistribute_interval", TimeSpan.FromMinutes(5) + tick),
                "rdy_redistribute_interval");
            //AssertHelper.Throws<Exception>(() => c.Set("client_id", "my"), "client_id");
            //AssertHelper.Throws<Exception>(() => c.Set("hostname", "my.host.name.com"), "hostname");
            //AssertHelper.Throws<Exception>(() => c.Set("user_agent", "user-agent/1.0"), "user_agent");
            //AssertHelper.Throws<Exception>(() => c.Set("heartbeat_interval", TimeSpan.MaxValue), "heartbeat_interval");
            AssertHelper.Throws<Exception>(() => c.Set("sample_rate", 99 + 1), "sample_rate");
            //AssertHelper.Throws<Exception>(() => c.Set("tls_config", tlsConfig);
            //AssertHelper.Throws<Exception>(() => c.Set("deflate", true), "deflate");
            AssertHelper.Throws<Exception>(() => c.Set("deflate_level", 9 + 1), "deflate_level");
            //AssertHelper.Throws<Exception>(() => c.Set("snappy", true), "snappy");
            //AssertHelper.Throws<Exception>(() => c.Set("output_buffer_size", Int64.MaxValue), "output_buffer_size");
            //AssertHelper.Throws<Exception>(() => c.Set("output_buffer_timeout", TimeSpan.MaxValue), "output_buffer_timeout");
            //AssertHelper.Throws<Exception>(() => c.Set("max_in_flight", int.MaxValue), "max_in_flight");
            //AssertHelper.Throws<Exception>(() => c.Set("msg_timeout", TimeSpan.MaxValue), "msg_timeout");
            //AssertHelper.Throws<Exception>(() => c.Set("auth_secret", "!@#@#$#%"), "auth_secret");
        }

        [TestMethod]
        public void TestHeartbeatLessThanReadTimeout()
        {
            var c = new NsqConfig();

            c.Set("read_timeout", "5m");
            c.Set("heartbeat_interval", "2s");
            c.Validate();

            c.Set("read_timeout", "2s");
            c.Set("heartbeat_interval", "5m");
            AssertHelper.Throws<Exception>(c.Validate);
        }

        [TestMethod]
        public void TestClone()
        {
            var c = new NsqConfig();

            var backoffStrategy = new FullJitterStrategy();
            var tlsConfig = new TlsConfig();
            c.Set("dial_timeout", "50ms");
            c.Set("read_timeout", "5m");
            c.Set("heartbeat_interval", "2s");
            c.Set("rdy_redistribute_interval", "3s");
            c.Set("backoff_strategy", backoffStrategy);
            c.Set("rdy_redistribute_on_idle", true);
            c.Set("tls_config", tlsConfig);
            c.Validate();

            var c2 = c.Clone();

            Assert.AreEqual(TimeSpan.FromMilliseconds(50), c2.DialTimeout, "dial_timeout");
            Assert.AreEqual(TimeSpan.FromMinutes(5), c2.ReadTimeout, "read_timeout");
            Assert.AreEqual(TimeSpan.FromSeconds(10), c2.WriteTimeout, "write_timeout");
            Assert.AreEqual(TimeSpan.FromSeconds(60), c2.LookupdPollInterval, "lookupd_poll_interval");
            Assert.AreEqual(0.3, c2.LookupdPollJitter, "lookupd_poll_jitter");
            Assert.AreEqual(TimeSpan.FromMinutes(15), c2.MaxRequeueDelay, "max_requeue_delay");
            Assert.AreEqual(TimeSpan.FromSeconds(90), c2.DefaultRequeueDelay, "default_requeue_delay");
            Assert.AreEqual(backoffStrategy, c2.BackoffStrategy, "backoff_strategy");
            Assert.AreEqual(TimeSpan.FromMinutes(2), c2.MaxBackoffDuration, "max_backoff_duration");
            Assert.AreEqual(TimeSpan.FromSeconds(1), c2.BackoffMultiplier, "backoff_multiplier");
            Assert.AreEqual(5, c2.MaxAttempts, "max_attempts");
            Assert.AreEqual(TimeSpan.FromSeconds(10), c2.LowRdyIdleTimeout, "low_rdy_idle_timeout");
            Assert.AreEqual(TimeSpan.FromSeconds(3), c2.RDYRedistributeInterval, "rdy_redistribute_interval");
            Assert.AreEqual(true, c2.RDYRedistributeOnIdle, "rdy_redistribute_on_idle");
            Assert.AreEqual(OS.Hostname().Split('.')[0], c2.ClientID, "client_id");
            Assert.AreEqual(OS.Hostname(), c2.Hostname, "hostname");
            Assert.AreEqual(string.Format("{0}/{1}", ClientInfo.ClientName, ClientInfo.Version), c2.UserAgent,
                "user_agent");
            Assert.AreEqual(TimeSpan.FromSeconds(2), c2.HeartbeatInterval, "heartbeat_interval");
            Assert.AreEqual(0, c2.SampleRate, "sample_rate");
            Assert.AreEqual(tlsConfig, c2.TlsConfig, "tls_config");
            //Assert.AreEqual(false, c2.Deflate, "deflate"); // TODO: Deflate
            //Assert.AreEqual(6, c2.DeflateLevel, "deflate_level"); // TODO: Deflate
            //Assert.AreEqual(false, c2.Snappy, "snappy"); // TODO: Snappy
            Assert.AreEqual(16384, c2.OutputBufferSize, "output_buffer_size");
            Assert.AreEqual(TimeSpan.FromMilliseconds(250), c2.OutputBufferTimeout, "output_buffer_timeout");
            Assert.AreEqual(1, c2.MaxInFlight, "max_in_flight");
            Assert.AreEqual(TimeSpan.Zero, c2.MessageTimeout, "msg_timeout");
            Assert.IsNull(c2.AuthSecret, "auth_secret");
        }

        [TestMethod]
        public void TestTls()
        {
            var c = new NsqConfig();
            c.Set("tls_insecure_skip_verify", true);

            Assert.IsNotNull(c.TlsConfig, "TlsConfig");
            Assert.IsTrue(c.TlsConfig.InsecureSkipVerify, "TlsConfig.InsecureSkipVerify");

            // default
            Assert.IsTrue(c.TlsConfig.CheckCertificateRevocation);

            c.Set("tls-check-certificate-revocation", false);
            Assert.IsFalse(c.TlsConfig.CheckCertificateRevocation);

            c.Set("tls-check-certificate-revocation", true);
            Assert.IsTrue(c.TlsConfig.CheckCertificateRevocation);

            // tls_min_version
            c.Set("tls_min_version", "ssl3.0");
            Assert.AreEqual(SslProtocols.Ssl3, c.TlsConfig.MinVersion);

            c.Set("tls_min_version", "tls1.0");
            Assert.AreEqual(SslProtocols.Tls, c.TlsConfig.MinVersion);

#if !NETFX_3_5 && !NETFX_4_0
            c.Set("tls_min_version", "tls1.1");
            Assert.AreEqual(SslProtocols.Tls11, c.TlsConfig.MinVersion);

            c.Set("tls_min_version", "tls1.2");
            Assert.AreEqual(SslProtocols.Tls12, c.TlsConfig.MinVersion);
#endif

            AssertHelper.Throws<Exception>(() => c.Set("tls_min_version", "ssl2.0"));

            AssertHelper.Throws<Exception>(() => c.Set("tls-min-version", "tls1.3"));
        }

        [TestMethod]
        public void TestBackoffStrategyCoerce()
        {
            var c = new NsqConfig();

            c.Set("backoff_strategy", "exponential");
            Assert.AreEqual(typeof(ExponentialStrategy), c.BackoffStrategy.GetType());

            c.Set("backoff_strategy", "");
            Assert.AreEqual(typeof(ExponentialStrategy), c.BackoffStrategy.GetType());

            c.Set("backoff_strategy", null);
            Assert.IsNull(c.BackoffStrategy);

            c.Set("backoff_strategy", "full_jitter");
            Assert.AreEqual(typeof(FullJitterStrategy), c.BackoffStrategy.GetType());

            AssertHelper.Throws<Exception>(() => c.Set("backoff_strategy", "invalid"));

            var fullJitterStrategy = new FullJitterStrategy();
            c.Set("backoff_strategy", fullJitterStrategy);
            Assert.AreEqual(fullJitterStrategy, c.BackoffStrategy);

            var exponentialStrategy = new ExponentialStrategy();
            c.Set("backoff_strategy", exponentialStrategy);
            Assert.AreEqual(exponentialStrategy, c.BackoffStrategy);

            AssertHelper.Throws<Exception>(() => c.Set("backoff_strategy", new object()));
        }

        [TestMethod]
        public void TestExponentialBackoff()
        {
            var expected = new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(8),
                TimeSpan.FromSeconds(32)
            };

            var backoffStrategy = new ExponentialStrategy();

            var config = new NsqConfig();
            config.BackoffMultiplier = TimeSpan.FromSeconds(1);

            var attempts = new[] { 1, 2, 4, 6 };
            for (var i = 0; i < attempts.Length; i++)
            {
                var result = backoffStrategy.Calculate(config, attempts[i]);

                Assert.AreEqual(expected[i], result.Duration,
                    string.Format("wrong backoff duration for attempt {0}", attempts[i]));
            }
        }

        [TestMethod]
        public void TestFullJitterBackoff()
        {
            // afaik there's no way to seed a RNGCryptoServiceProvider (probably a good thing)
            // we'll test the intent: the range will be between 0 and 2^n*backoffmultiplier
            var maxExpected = new[]
            {
                TimeSpan.FromSeconds(0.5),
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(4),
                TimeSpan.FromSeconds(16)
            };

            var backoffStrategy = new FullJitterStrategy();

            var config = new NsqConfig();
            config.BackoffMultiplier = TimeSpan.FromSeconds(0.5);

            var attempts = new[] { 1, 2, 4, 6 };
            for (var count = 0; count < 50000; count++)
                for (var i = 0; i < attempts.Length; i++)
                {
                    var attempt = attempts[i];
                    var result = backoffStrategy.Calculate(config, attempt).Duration;

                    AssertHelper.LessOrEqual(result, maxExpected[i],
                        string.Format("wrong backoff duration for attempt {0}", attempt));
                }
        }

        [TestMethod]
        public void TestExponentialMaxBackoffLevel()
        {
            TestBackoffMaxLevel(new ExponentialStrategy());
        }

        [TestMethod]
        public void TestFullJitterMaxBackoffLevel()
        {
            TestBackoffMaxLevel(new FullJitterStrategy());
        }

        private void TestBackoffMaxLevel(IBackoffStrategy backoffStrategy)
        {
            for (var maxBackoff = 0; maxBackoff <= 16; maxBackoff++)
                for (var multiplier = 1; multiplier <= 16; multiplier++)
                {
                    var config = new NsqConfig
                    {
                        MaxBackoffDuration = TimeSpan.FromMilliseconds(maxBackoff),
                        BackoffMultiplier = TimeSpan.FromMilliseconds(multiplier)
                    };
                    config.Validate();

                    var expectedMaxLevel = 1;
                    if (maxBackoff != 0 && multiplier != 0)
                    {
                        var x = config.MaxBackoffDuration.TotalSeconds / config.BackoffMultiplier.TotalSeconds;
                        expectedMaxLevel = (int)Math.Ceiling(Math.Log(x, 2)) + 1;
                        if (expectedMaxLevel <= 0)
                            expectedMaxLevel = 1;
                    }

                    bool increaseBackoffLevel;
                    if (expectedMaxLevel > 1)
                    {
                        increaseBackoffLevel = backoffStrategy.Calculate(config, expectedMaxLevel - 1).IncreaseBackoffLevel;

                        Assert.IsTrue(increaseBackoffLevel,
                            string.Format("increaseBackoff max={0} multiplier={1} level={2} expectedMaxLevel={3}",
                                config.MaxBackoffDuration, config.BackoffMultiplier, expectedMaxLevel - 1,
                                expectedMaxLevel));
                    }

                    increaseBackoffLevel = backoffStrategy.Calculate(config, expectedMaxLevel).IncreaseBackoffLevel;

                    Assert.IsFalse(increaseBackoffLevel,
                        string.Format("increaseBackoff max={0} multiplier={1} level={2} expectedMaxLevel={3}",
                            config.MaxBackoffDuration, config.BackoffMultiplier, expectedMaxLevel, expectedMaxLevel));
                }
        }
    }
}