﻿/*
 * libBeanstalk.NET 
 * Copyright (C) 2011 Arne F. Claassen
 * geekblog [at] claassen [dot] net
 * http://github.com/sdether/libBeanstalk.NET 
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.IO;
using System.Text;
using Droog.Beanstalk.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Beanstalk.Client.Json {
    public static class JsonExtensions {
        public static PutResponse PutJson(this IBeanstalkClient client, JObject data) {
            using(var stream = GetJsonStream(data)) {
                return client.Put(client.Defaults.Priority, client.Defaults.Delay, client.Defaults.TimeToRun, stream, stream.Length);
            }
        }

        public static PutResponse PutJson(this IBeanstalkClient client, JObject data, uint priority) {
            using(var stream = GetJsonStream(data)) {
                return client.Put(priority, client.Defaults.Delay, client.Defaults.TimeToRun, stream, stream.Length);
            }
        }

        public static PutResponse PutJson(this IBeanstalkClient client, JObject data, uint priority, TimeSpan delay) {
            using(var stream = GetJsonStream(data)) {
                return client.Put(priority, delay, client.Defaults.TimeToRun, stream, stream.Length);
            }
        }

        public static PutResponse PutJson(this IBeanstalkClient client, JObject data, uint priority, TimeSpan delay, TimeSpan timeToRun) {
            using(var stream = GetJsonStream(data)) {
                return client.Put(priority, delay, timeToRun, stream, stream.Length);
            }
        }

        public static Job<JObject> ReserveJson(this IBeanstalkClient client) {
            var job = client.Reserve();
            using(var reader = new StreamReader(job.Data)) {
                using(var jsonReader = new JsonTextReader(reader)) {
                    return new Job<JObject>(job.Id, JObject.Load(jsonReader));
                }
            }
        }

        public static Job<JObject> ReserveJson(this IBeanstalkClient client, TimeSpan timeout) {
            var job = client.Reserve(timeout);
            if(job == null) {
                return null;
            }
            using(var reader = new StreamReader(job.Data)) {
                using(var jsonReader = new JsonTextReader(reader)) {
                    return new Job<JObject>(job.Id, JObject.Load(jsonReader));
                }
            }
        }

        private static MemoryStream GetJsonStream(JObject data) {
            return new MemoryStream(Encoding.UTF8.GetBytes(data.ToString()));
        }
    }
}
