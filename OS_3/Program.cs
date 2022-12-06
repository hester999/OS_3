using System.Collections;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

class Program
{
     static void  Main(string[] args)
     {
          Print m = new Print();
          
          m.menu();

     }

}

class Print
{
     public static bool flag = false;
     public static Queue<int> q = new Queue<int>(200);
     
     public static bool stop = false;
     
     
     public void menu()
     {
          
          
          Channel<int> channel = Channel.CreateUnbounded<int>();  
          
          var prod = Task.Run(() => { new Producer(channel.Writer); });
        
          Task[] streams = new Task[4 + 1];
        
          streams[0] = prod;
        
          for (int i = 1; i < 4 + 1; i++)
          {
               streams[i] = Task.Run(() => { new Consumer(channel.Reader, Print.q.Count); });
          }

          var stop = Task.Run(() =>
          {
             
               var res =  Console.ReadLine() ;
               if (res == "q")
               {
                    Print.stop = true;
                    
                    return;
               }
          });
          Task.WaitAny(streams);

          
     }
}

class Producer
{
     static  public ChannelWriter<int> Writer;

     public Producer(ChannelWriter<int> _writer)
     {
          Writer = _writer;

          Task.WaitAll(A());

     }

     public async Task A()
     {
          while (await Writer.WaitToWriteAsync())
          {
               
              
               while (true)
               {
                    if (Print.stop)
                    {
                         Console.WriteLine("Строка остановилась");
                         Writer.Complete();
                         return;
                    }
                    if ((Print.q.Count() <=80)&&(Print.q.Count()< 100) && (!Print.flag))
                    {
                         
                         for (int i = 0; i <= 100; i++)
                         {
                              var rand = new Random();
                              int value = rand.Next(1, 101);
                              
                              Thread.Sleep(100);
                              Print.q.Enqueue(value);
                              Writer.WriteAsync(value);
                              
                             
                              foreach (int num in Print.q)
                              {
                                   
                                   Console.WriteLine($"Элемент очереди, который не изъят - {num}");
                                   
                                   if (Print.stop)
                                   {
                                      
                              
                                        return;
                                   }
                              }
                              
                              if (Print.q.Count() >= 100 && (!Print.flag))
                              {
                                   Console.WriteLine($"lenght queue writer - {Print.q.Count} ");
                         
                         
                                   Console.WriteLine("stop stream Writer");
                                   Thread.Sleep(20000);
                       
                              }
                              
                         }
                       
                          
                    }
                    
                     
                    
               }
               
          } 
     }
}



class Consumer
{
     private ChannelReader<int> Reader;

     private int num;
     
     
     public Consumer(ChannelReader<int> _reader, int _num)
     {
          Reader = _reader;
          num = _num;
          Task.WaitAll(Run());

     }
     
     async Task Run()
     {
          while (await Reader.WaitToReadAsync())
          {
               while (true)
               {
                    if (!Print.flag)
                    {
                         var item = await Reader.ReadAsync();
                    
                    
                
                         if ((Print.q.Count()>=80) && (!Print.flag))
                         {
                           
                              Thread.Sleep(1000);
                              Console.WriteLine(Print.q.Dequeue());
                              Console.WriteLine($"Элементов после извлечения  - {Print.q.Count} ");
                              
                        
                         }
                         else if (Print.q.Count <= 80 && (!Print.flag))
                         {
                              Console.WriteLine($"Осталось элементов в очереди  - {Print.q.Count} ");
                         
                              
                              Console.WriteLine("stop stream Reader");
                              Thread.Sleep(10000);
                       
                         }
                    }
                    else return; 
               }
              
          }
     }
}