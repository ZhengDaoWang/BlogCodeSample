// See https://aka.ms/new-console-template for more information


using AdapterSample;

var xiaoYezi = new XiaoYezi();
var shiYiAdapter = new ShiYiAdapter(xiaoYezi);
shiYiAdapter.Bark();
shiYiAdapter.ActingCute();
shiYiAdapter.Demolition();

Console.WriteLine("---------------- \n");

var shiYi = new ShiYi();
shiYiAdapter = new ShiYiAdapter(shiYi);
shiYiAdapter.Bark();
shiYiAdapter.ActingCute();
shiYiAdapter.Demolition();

Console.WriteLine("---------------- \n");

 xiaoYezi = new XiaoYezi();
var xiaoYeziAdapter = new XiaoYeziAdapter(xiaoYezi);
xiaoYeziAdapter.Bark();
xiaoYeziAdapter.Eat();
xiaoYeziAdapter.HerdingChicken();

Console.WriteLine("---------------- \n");

 shiYi = new ShiYi();
xiaoYeziAdapter = new XiaoYeziAdapter(shiYi);
xiaoYeziAdapter.Bark();
xiaoYeziAdapter.Eat();
xiaoYeziAdapter.HerdingChicken();


xiaoYezi = new XiaoYezi();
var daHaFamilyDogAdapter = new DaHaFamilyDogAdapter(xiaoYezi);
daHaFamilyDogAdapter.Bark();
daHaFamilyDogAdapter.Eat();
daHaFamilyDogAdapter.HerdingChicken();
daHaFamilyDogAdapter.ActingCute();

Console.WriteLine("---------------- \n");

shiYi = new ShiYi();
daHaFamilyDogAdapter = new DaHaFamilyDogAdapter(shiYi);
daHaFamilyDogAdapter.Bark();
daHaFamilyDogAdapter.Eat();
daHaFamilyDogAdapter.HerdingChicken();
daHaFamilyDogAdapter.ActingCute();
