using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Car_Seller.models
{
    public class MyEnums
    {
        public enum BodyType
        {
            [Description("не выбрано")]
            none,
            [Description("седан")]
            sedan,
            [Description("двухдверный седан")]
            sedan_two_door,
            [Description("универсал")]
            universal,
            [Description("хэтчбек")]
            hatchback,
            [Description("купе")]
            coupe,
            [Description("лимузин")]
            limousine,
            [Description("микроавтобус")]
            minibus,
            [Description("минивэн")]
            minivan,
            [Description("хардтоп")]
            hardtop,
            [Description("таункар")]
            tawncar,
            [Description("комби")]
            combi,
            [Description("лифтбэк")]
            liftback,
            [Description("фастбэк")]
            fastback,
            [Description("кабриолет")]
            cabriolet,
            [Description("родстер")]
            roadster,
            [Description("фаэтон")]
            phaeton,
            [Description("ландо")]
            lando,
            [Description("седан")]
            brogam,
            [Description("тарга")]
            targa,
            [Description("спайдер")]
            spider,
            [Description("шутингбрейк")]
            shootingbrake,
            [Description("пикап")]
            pickup,
            [Description("ют")]
            ut,
            [Description("фургон")]
            van,
            [Description("внедорожник")]
            offroad
        }

        public enum TransmissionType
        {
            [Description("не выбрано")]
            none,
            [Description("механическая")]
            mechanic,
            [Description("автоматическая")]
            automate
        }

        public enum EngineType
        {
            [Description("не выбрано")]
            none,
            [Description("газовый")]
            gas,
            [Description("электрический")]
            electric,
            [Description("дизельный")]
            diesel,
            [Description("бензиновый")]
            gasoline
        }

        public enum DriveType
        {
            [Description("не выбрано")]
            none,
            [Description("задний")]
            back,
            [Description("передний")]
            front,
            [Description("полный")]
            full
        }

        public class Body
        {
            private BodyType _type;

            public Body(BodyType type)
            {
                _type = type;
            }

            public Body(string str)
            {
                _type = FromString<BodyType>(str);
            }

            public static implicit operator Body(string x)
            {
                return new Body(x);
            }
            public static explicit operator string(Body counter)
            {
                return counter.ToString();
            }

            public override string ToString()
            {
                return MyEnums.ToString(_type);
            }
        }

        public class Engine
        {
            private EngineType _type;

            public Engine(EngineType type)
            {
                _type = type;
            }
            public Engine(string str)
            {
                _type = FromString<EngineType>(str);
            }

            public static implicit operator Engine(string x)
            {
                return new Engine(x);
            }
            public static explicit operator string(Engine counter)
            {
                return counter.ToString();
            }

            public override string ToString()
            {
                return MyEnums.ToString(_type);
            }
        }

        public class Transmission
        {
            private TransmissionType _type;

            public Transmission(TransmissionType type)
            {
                _type = type;
            }
            public Transmission(string str)
            {
                _type = FromString<TransmissionType>(str);
            }

            public static implicit operator Transmission(string x)
            {
                return new Transmission(x);
            }
            public static explicit operator string(Transmission counter)
            {
                return counter.ToString();
            }

            public override string ToString()
            {
                return MyEnums.ToString(_type);
            }
        }

        public class Drive
        {
            private DriveType _type;

            public Drive(DriveType type)
            {
                _type = type;
            }
            public Drive(string str)
            {
                _type = FromString<DriveType>(str);
            }


            public static implicit operator Drive(string x)
            {
                return new Drive(x);
            }
            public static explicit operator string(Drive counter)
            {
                return counter.ToString();
            }

            public override string ToString()
            {
                return MyEnums.ToString(_type);
            }
        }

        public static string ToString(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
        
        public static T FromString<T>(string description)
        {
            foreach (var field in typeof(T).GetFields())
            {
                var descriptions = (DescriptionAttribute[])
                       field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (descriptions.Any(x => x.Description == description))
                {
                    return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Не найдено такого объекта перечисления");
        }
    }
}
