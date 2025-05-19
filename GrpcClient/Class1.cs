namespace GrpcClient
{
    public  class Class1
    {
        public byte Ctype = 0;

        public virtual int hz(ref string ololo, ref int tt)
        {



            tt = 1;
            ololo = "shit";
            


            Class2 class2 = new Class2();
            Class3 class3 = new Class3();
            Class4 class4 = new Class4();
            Class1 class1 = new Class1();

            List<Class1> Tatata = new List<Class1>()
            {class2, class3, class4, class1 };
            //List<Class2> Tatata = new List<Class2>()
            //{class2, class3, class4, class1 };

            foreach (Class1 c in Tatata)
            {
                Type type = c.GetType();
                string hz = type.Name;//ток так пока или if-ы
                switch (hz)
                {
                    case "Class1":
                        break;
                    case "Class2":
                        break;
                    case "Class3":
                        break;
                    case "Class4":
                        break;
                    case "Class5":
                        break;



                }

            }
            return 0;
        }


        

        public void MainHz()
        {
            int tt = 2;
            string ololo = null;

            var shit = hz(ref  ololo, ref tt );


        }

    }
    public class Class2 : Class1
    {
        
        public Class2()
        {



        }
         
        public override  int hz(ref string ololo, ref int tt)
        {
            tt = 1;
            ololo = "shit";
            return 0;


        }





    }
    public class Class3 : Class1
    {

        public string hzzzz;
        public override int hz(ref string ololo, ref int tt)
        {
            tt = 1;
            ololo = "shit";
            return 0;


        }



    }
    public class Class4 : Class1
    {
        public int hzzzz;
        public override int hz(ref string ololo, ref int tt)
        {
            tt = 1;
            ololo = "shit";
            return 0;


        }



    }
}
