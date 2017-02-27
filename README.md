# WPF制作的wnmp环境

[![Build Status](https://travis-ci.org/slimphp/Slim.svg?branch=develop)](https://travis-ci.org/slimphp/Slim)
[![Coverage Status](https://coveralls.io/repos/slimphp/Slim/badge.svg)](https://coveralls.io/r/slimphp/Slim)
[![Total Downloads](https://poser.pugx.org/slim/slim/downloads)](https://packagist.org/packages/slim/slim)
[![License](https://poser.pugx.org/slim/slim/license)](https://packagist.org/packages/slim/slim)

## 集成包下载
[下载地址](http://pan.baidu.com/s/1skJzyLR)

## UI界面
![SalamanderWnmp](http://git.oschina.net/uploads/images/2017/0222/202019_652ee6f0_433553.png)
![编程面板](http://git.oschina.net/uploads/images/2017/0222/160849_e369b9e1_433553.png)


## 注意
php 版本为7.0.1 64位版本，需要MSVC14 (Visual C++ 2015)运行库支持，下载：https://download.microsoft.com/download/9/3/F/93FCF1E7-E6A4-478B-96E7-D4B285925B00/vc_redist.x64.exe



## 配置

集成包下有文件：配置.txt
### php
修改php.ini中的extension_dir配置，改为php目录下ext目录的绝对路径

### nginx

修改conf目录下nginx.conf中的localhost虚拟主机的root（网站根目录），改为某个目录（譬如D:/web_root）的绝对路径（使用/，使用\会出错）

### mysql

修改my.ini文件中base_dir和data_dir的配置，分别修改为mysql目录和mysql目录下的data目录的绝对路径




