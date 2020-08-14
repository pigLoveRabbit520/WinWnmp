# WNMP Based On WPF(WPF制作的wnmp环境)

[![Build Status](https://travis-ci.org/slimphp/Slim.svg?branch=develop)](https://travis-ci.org/slimphp/Slim)
[![Coverage Status](https://coveralls.io/repos/slimphp/Slim/badge.svg)](https://coveralls.io/r/slimphp/Slim)
[![License](https://poser.pugx.org/slim/slim/license)](https://packagist.org/packages/slim/slim)

## Download(集成包下载)
* [下载地址(Nginx)](http://file.51lucy.com/SalamanderWnmp.7z)
* [下载地址(OpenResty)](http://file.51lucy.com/SalamanderWnmp-OpenResty.7z)

## Main Window(主界面)
<img src="https://user-images.githubusercontent.com/16663435/33982852-0c885306-e0ed-11e7-98ad-6c44f32d1598.png" width="420" alt="主界面">


## Usage(使用)
1. 下载集成包后，运行SalamanderWnmp.exe（首次运行会自动配置）
2. 开启nginx和php，然后在浏览器中输入http://localhost/  即可看到`Hello Salamander！`字样，说明配置成功。（使用数据库功能，则打开mysql）


## Notice(注意)
*   php 版本为7.3.21 64位版本，需要MSVC15 (Visual C++ 2017)运行库支持，[下载](https://aka.ms/vs/16/release/vc_redist.x64.exe)
*   MySQL的默认账户为root，密码为空


## Other Functions(其他功能)
#### Regulaer Settings(常规设置)
<img src="https://user-images.githubusercontent.com/16663435/30427886-22070566-9984-11e7-9fc0-9900064b7d71.png" width="420" alt="常规设置">


#### Theme Color Panel(调色面板)
<img src="https://cloud.githubusercontent.com/assets/16663435/23488548/4fcc4b6a-ff28-11e6-8a1c-cf45b961340d.png" width="420" alt="调色面板">


#### Coding Panel(编程面板)
<img src="https://user-images.githubusercontent.com/16663435/32403367-ab504c9a-c172-11e7-9831-ac2d645cb09e.jpg" width="420" alt="编程面板">

#### Questions Panel(常见问题面板)
<img src="https://cloud.githubusercontent.com/assets/16663435/25732424/fc42c0f0-3181-11e7-9ebf-c9cd1eba747e.png" width="420" alt="常见问题面板">


## Switch PHP Version(切换PHP版本)
1. 从php官网下载某个版本的PHP，放入你的wnmp目录中（这里假设下载了PHP5.6，可以命名为php5.6）
2. 修改php.ini（这个文件可以从php.ini-development或php.ini-production重命名过来）中extension_dir选项，改为wnmp中php5.6的ext目录的绝对路径（举例：D:/SalamanderWnmp/php5.6/ext），另外顺便把一些扩展也加载进来（去掉前面分号即可）
```
extension=php_bz2.dll
extension=php_curl.dll
extension=php_fileinfo.dll
extension=php_ftp.dll
extension=php_gd2.dll
extension=php_gettext.dll
```
3. wnmp软件修改php目录配置

<img src="https://user-images.githubusercontent.com/32063728/30805849-f95c3780-a225-11e7-9210-a0f34e54358c.png" width="420" alt="修改php目录">

4. 重启PHP


## Third Party Libraries(第三方库)
*	[AvalonEdit](https://github.com/icsharpcode/AvalonEdit)
*   [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis)



## Contact Me(联系我)
*	[Blog](http://blog.51lucy.com/)
*	[segmentfault](https://segmentfault.com/u/salamander)

## Sponsor（赞助）
#### 支付宝  
<img src="/support_img/alipay.jpg" width="200" alt="支付宝">

#### 微信支付

<img src="/support_img/wxpay.jpg" width="200" alt="微信支付">
