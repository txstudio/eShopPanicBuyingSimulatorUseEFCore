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
            args = new string[] { "-t", "100" };

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
            bool _hasStorage;
            short _quantity;
            int _maxValue;
            int _productIndex;

            Random _random;
            Guid _memberGUID;
            List<OrderDetail> _items;

            _hasStorage = false;
            _random = new Random();

            while (true)
            {
                ProductService _productService;
                OrderService _orderService;

                _productService = new ProductService();
                _orderService = new OrderService();

                _memberGUID = Guid.NewGuid();
                _hasStorage = false;

                var _products = _productService.GetProductList();
                

                //判斷所有商品是否都有庫存
                foreach (var _item in _products)
                {
                    var _storage = _productService.GetStorage(_item.Schema);

                    if(_storage > 0)
                    {
                        _hasStorage = true;
                        break;
                    }
                }

                if (_hasStorage == false)
                    break;


                //隨機取得要購買的商品編號
                _quantity = Convert.ToInt16(_random.Next(1, 3));
                _maxValue = _products.Count();
                _productIndex = _random.Next(0, _maxValue);

                var _product = _products.ToArray()[_productIndex];
                               
                _items = new List<OrderDetail>();
                _items.Add(new OrderDetail()
                {
                    ProductNo = _product.No,
                    Quantity = _quantity,
                    SellPrice = _product.SellPrice
                });

                var _OrderResult = _orderService.AddOrder(_memberGUID, _items);
            }

            //執行完畢
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

                Console.WriteLine();
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

}