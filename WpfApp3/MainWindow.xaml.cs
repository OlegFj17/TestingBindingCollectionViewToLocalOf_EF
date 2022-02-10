using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfApp3
{
    public class Shop
    {
        public int ShopId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
    public class DbCtx: DbContext
    {
        public DbSet<Shop> Shops { get; set; }
        public DbCtx()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AAA3;Trusted_Connection=True;");
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shop>().HasData(new Shop[]{
                new Shop{ShopId=1, Name="Shop1", Address="AAA"},
                new Shop{ShopId=2, Name="Shop2", Address="BBB"},
                new Shop{ShopId=3, Name="Shop3", Address="CCC"},
                new Shop{ShopId=4, Name="Shop4", Address="DDD"},
                new Shop{ShopId=5, Name="Shop5", Address="EEE"}
            });
            base.OnModelCreating(modelBuilder);
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DbCtx Db {get;}
        public ObservableCollection<Shop> ShopsCollection { get; set; }
        public ICollectionView ShopsView { get; set; }
        public MainWindow()
        {

            Db = new DbCtx();
            ShopsCollection = Db.Shops.Local.ToObservableCollection();
            ShopsView = CollectionViewSource.GetDefaultView(ShopsCollection);
            
            //
            Db.Shops.FirstOrDefault(s => s.Name == "Shop1");

            DataContext = this;
            InitializeComponent();
        }

        private void Button_Sync(object sender, RoutedEventArgs e)
        {
            try
            {
                //закачиваем следующую запись, это изменяет Local-коллекцию
                //но в главном потоке, поэтому всё ок.
                Db.Shops.FirstOrDefault(s => s.Name == "Shop2");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{ex}");
            }            
        }

        private async void Button_Async(object sender, RoutedEventArgs e)
        {
            try
            {
                //закачиваем следующую запись асинхронно, это изменяет Local-коллекцию через другой поток
                //поэтому получаем исключение, что SourceCollection для ICollectionView не может быть изменен
                //из потока отличного от главного(UI)
                await Db.Shops.FirstOrDefaultAsync(s => s.Name == "Shop3");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{ex.Message}");
            }
        }

        private async void Button_Async_Dispatcher(object sender, RoutedEventArgs e)
        {
            //имитируем какую-то дополнительную работу асинхронного метода
            await Task.Delay(3000).ConfigureAwait(false);
            try
            {
                //здесь по сути происходит синхронное обращение к БД из UI
                //если запрос большой, то UI виснет
                //если отключить БД("нет соединения"), то UI виснет ровно на 10 секунд и только потом
                //выводит исключение, что БД не доступна
                await Application.Current.Dispatcher.InvokeAsync(
                    () => Db.Shops.FirstOrDefault(s => s.Name == "Shop4"));                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{ex.Message}");
            }
        }
    }
}
