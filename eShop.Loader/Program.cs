using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace eShop.Loader
{
    class Program
    {

        static LoaderOptions _option = new LoaderOptions();
        static Random _random = new Random();
        static bool _exit = false;


        static void Main(string[] args)
        {
            //設定 task 數量為 5 個
            _option.TaskNumber = "5";

            SetArgs(_option, args);

            //初始化資料庫內容
            InitDatabase();

            List<Task> _tasks;

            _tasks = new List<Task>();

            for (int i = 0; i < _option.Task; i++)
                _tasks.Add(new Task(eShopBuyer));

            for (int i = 0; i < _option.Task; i++)
                _tasks[i].Start();

            Stopwatch _stopwatch = new Stopwatch();

            while (_exit == false)
            {
                _exit = true;

                for (int i = 0; i < _option.Task; i++)
                {
                    if (_tasks[i].Status == TaskStatus.Running)
                        _stopwatch.Start();

                    if (_tasks[i].Status == TaskStatus.Running 
                        || _tasks[i].Status == TaskStatus.WaitingToRun)
                    {
                        Thread.Sleep(100);
                        _exit = false;
                        continue;
                    }
                }

                if (_exit == false)
                    continue;

                _exit = true;
                _stopwatch.Stop();

                Thread.Sleep(1000);
            }

            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }


        static void eShopBuyer()
        {
            var _memberGUID = Guid.NewGuid();

            using (eShopContext _db = new eShopContext())
            {
                eShopRepository _eShop = new eShopRepository(_db);

                var _products = _eShop.GetProducts().ToList();

                bool _orderResult;
                short _quantity;
                int _maxValue;
                int _productIndex;
                int _validStorage;

                ProductMain _product;
                List<OrderDetail> _items;
                StringBuilder _builder;

                _builder = new StringBuilder();

                while(true)
                {
                    _quantity = Convert.ToInt16(_random.Next(1, 3));
                    _maxValue = _products.Count();
                    _productIndex = _random.Next(0, _maxValue);

                    _product = _products[_productIndex];

                    _validStorage = _eShop.GetProductValidStorage(_products[0].Schema);
                                       
                    //如果所有商品都沒有庫存的話取消訂購
                    if (_validStorage <= 0)
                    {
                        _validStorage = _eShop.GetProductValidStorage(_products[1].Schema);

                        if (_validStorage <= 0)
                        {
                            _validStorage = _eShop.GetProductValidStorage(_products[2].Schema);

                            if (_validStorage <= 0)
                            {
                                _builder.Clear();
                                _builder.AppendFormat("會員 {0} 完成作業", _memberGUID);

                                //_eShop.AddEventBuying(_memberGUID, _builder.ToString(), true);

                                Console.WriteLine(_builder.ToString());
                                break;
                            }
                        }
                    }

                    _items = new List<OrderDetail>();
                    _items.Add(new OrderDetail()
                    {
                        ProductNo = _product.No,
                        Quantity = _quantity,
                        SellPrice = _product.SellPrice
                    });

                    _orderResult = _eShop.AddOrder(_memberGUID, _items);

                    //訂購商品
                    _builder.Clear();
                    _builder.AppendFormat("會員 {0} 訂購商品 {1} {2} 個，訂購 "
                                            , _memberGUID
                                            , _product.Name
                                            , _quantity);

                    if (_orderResult == true)
                        _builder.Append("成功 ...");
                    else
                        _builder.Append("失敗 ...");

                    //_eShop.AddEventBuying(_memberGUID, _builder.ToString(), _orderResult);
                    Console.WriteLine(_builder.ToString());
                }
            }
        }


        static void InitDatabase()
        {
            using (eShopContext _db = new eShopContext())
            {
                Console.WriteLine();
                Console.WriteLine("刪除 - 資料庫");

                _db.Database.EnsureDeleted();

                Console.WriteLine("新增 - 資料庫");

                _db.Database.EnsureCreated();

                Console.WriteLine("資料庫初始化完成 !");
                Console.WriteLine();

                ProductMain _productMain;

                _productMain = new ProductMain();
                _productMain.Schema = "DYAJ93A900930IK";
                _productMain.Name = "Microsoft Surface Pro (Core i7/16G/256G/W10P)";
                _productMain.SellPrice = 70888;
                _productMain.ProductStorage = new ProductStorage() { Storage = 75 };
                _db.ProductMains.Add(_productMain);

                _productMain = new ProductMain();
                _productMain.Schema = "DYAJ93A900929IK";
                _productMain.Name = "Microsoft Surface Pro (Core i5/8G/128G/W10P)";
                _productMain.SellPrice = 51888;
                _productMain.ProductStorage = new ProductStorage() { Storage = 75 };
                _db.ProductMains.Add(_productMain);

                _productMain = new ProductMain();
                _productMain.Schema = "DYAJ93A900928IK";
                _productMain.Name = "Microsoft Surface Pro (Core i3/4G/128G/W10P)";
                _productMain.SellPrice = 41888;
                _productMain.ProductStorage = new ProductStorage() { Storage = 75 };
                _db.ProductMains.Add(_productMain);

                _db.SaveChanges();

                Console.WriteLine("資料內容初始化完成 !");
                Console.WriteLine();
            }
        }

        static void SetArgs(LoaderOptions option, string[] args)
        {
            var _arg = string.Empty;
            var _index = 0;

            for (int i = 0; i < args.Length; i++)
            {
                _arg = args[i];
                _index = i + 1;

                if (_index <= args.Length)
                {
                    switch (_arg)
                    {
                        case "-t":
                            option.TaskNumber = args[_index];
                            break;
                        default:
                            break;
                    }
                }
            }

            Console.WriteLine("-------------------------");
            Console.WriteLine("Task 資訊");
            Console.WriteLine("-------------------------");
            Console.WriteLine("起始時間:{0}\t總數:{1}"
                            , option.StartTime
                            , option.Task);

        }
    }


    public class UnitOfWork : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

}