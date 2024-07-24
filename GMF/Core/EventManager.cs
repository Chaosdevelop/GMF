using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BaseCore;

namespace GMF
{

    public interface IEvent
    {

    }

    public interface IEventSubscriber
    {

    }

    public interface IEventSubscriber<T> : IEventSubscriber where T : struct, IEvent
    {
        public void ReceiveEvent(T data);
    }


    public static class EventManager
    {
        private static ConcurrentDictionary<Type, List<IEventSubscriber>> Subjects { get; } = new ConcurrentDictionary<Type, List<IEventSubscriber>>();
        private static ConcurrentDictionary<Type, List<Type>> Interfaces { get; } = new ConcurrentDictionary<Type, List<Type>>();


        static EventManager()
        {
            var types = TypeUtility.GetAllTypesDerivedFrom(typeof(IEvent));
            foreach (var type in types)
            {
                Subjects.TryAdd(type, new List<IEventSubscriber>());

            }

            var subtypes = TypeUtility.GetAllTypesDerivedFrom(typeof(IEventSubscriber));

            foreach (var subType in subtypes)
            {
                var i = TypeUtility.CreateInterfaces(subType);

                foreach (var item in i)
                {
                    if (!Interfaces.ContainsKey(subType))
                    {
                        Interfaces.TryAdd(subType, new List<Type>());
                    }
                    Interfaces[subType].Add(item.GenericArgument);

                }
            }
        }

        public static void UnSubscribeAll()
        {
            foreach (var item in Subjects)
            {
                item.Value.Clear();
            }
        }

        public static void Subscribe(IEventSubscriber subscriber)
        {
            var subType = subscriber.GetType();

            List<System.Type> i;
            Interfaces.TryGetValue(subType, out i);

            foreach (var item in i)
            {
                Subjects[item].Add(subscriber);
            }
        }


        static void SendToSubscribers<T>(List<IEventSubscriber> subscribersList, T data) where T : struct, IEvent
        {
            var matchedSubscribers = subscribersList.ToArray();
            for (int i = 0; i < matchedSubscribers.Length; i++)
            {
                (matchedSubscribers[i] as IEventSubscriber<T>).ReceiveEvent(data);
            }
        }

        public static void SendEvent<T>(T eventData) where T : struct, IEvent
        {
            var subType = typeof(T);
            var t = Subjects[subType];

            SendToSubscribers<T>(t, eventData);
        }


        public static void Unsubscribe(IEventSubscriber subscriber)
        {
            var subType = subscriber.GetType();
            List<System.Type> i;
            Interfaces.TryGetValue(subType, out i);

            foreach (var item in i)
            {
                Subjects[item].Remove(subscriber);
            }
        }

        public static void SelfSubscribe(this IEventSubscriber subscriber)
        {
            Subscribe(subscriber);
        }

        public static void SelfUnsubscribe(this IEventSubscriber subscriber)
        {
            Unsubscribe(subscriber);
        }
    }



}


