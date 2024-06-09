<!-- INTRO -->
<br />
<div align="center">
  <h1 align="center">Tugas Besar 3 IF2211 Strategi Algoritma Tahun Ajaran 2023/2024</h1>

  <p align="center">
    <h3> Pemanfaatan Pattern Matching dalam Membangun Sistem Deteksi Individu Berbasis Biometrik Melalui Citra Sidik Jari </h3>
    <p>Program made using C# Language</p>
    <br />
    <a href="https://github.com/zultopia/Tubes3_HapHipHop.git">Report Bug</a>
    ·
    <a href="https://github.com/zultopia/Tubes3_HapHipHop.git">Request Feature</a>
<br>
<br>

[![MIT License][license-shield]][license-url]

  </p>
</div>

<!-- CONTRIBUTOR -->
<div align="center" id="contributor">
  <strong>
    <h3>Dibuat oleh Kelompok "HapHipHop" :</h3>
    <table align="center">
      <tr>
        <td>NIM</td>
        <td>Nama</td>
      </tr>
      <tr>
        <td>Marzuli Suhada M</td>
        <td>13522070</td>
     </tr>
     <tr>
        <td>Ahmad Mudabbir Arif</td>
        <td>13522072</td>
    </tr>
     <tr>
        <td>Naufal Adnan</td>
        <td>13522116</td>
    </tr>
    </table>
  </strong>
</div>

## Table of Contents
1. [Deskripsi Program](#deskripsi-program)
2. [Bonus Enkripsi](#bonus-enkripsi)
3. [Bonus Video](#bonus-video) 
4. [Features](#features)
5. [Getting Started](#getting-started)
6. [How-to-Run](#how-to-run)
7. [Tampilan Program](#tampilan)

## Deskripsi Program

Program ini merupakan desktop app program yang dibuat dengan bahasa C#. Pada program ini, pengguna dapat memasukkan berkas citra berisi potongan gambar sidik jari. Nantinya akan dicari biodata dan sidik jari yang cocok dengan potongan gambar tersebut dengan memilih penggunaan algoritma antara Knuth Morris Prath dan Boyer Moore. Pencarian data yang cocok menggunakan pattern matching. Biodata pemilik sidik-jari bertujuan untuk mengetahui data informasi seperti yang tertera pada KTP. 
Dokumentasi lengkap tentang program dapat dilihat pada [link berikut](https://docs.google.com/document/d/1BHsjquNVL7-zK9WrRMB3IXFajNfWBN4aMshg5CqlR2Q/edit?usp=sharing)
   
## Bonus Enkripsi
Dalam aplikasi program yang kami buat, kami melakukan enkripsi data pada tabel biodata. Enkripsi bertujuan untuk mengamankan informasi pribadi terkait orangnya. Enkripsi ini diimplementasikan menggunakan algoritma AES. Algoritma AES (Advanced Encryption Standard) adalah metode enkripsi blok simetrik yang digunakan untuk mengamankan data digital, bekerja dengan blok data 128 bit dan mendukung panjang kunci 128, 192, dan 256 bit. AES dikenal karena keamanan tinggi, kinerja cepat, fleksibilitas panjang kunci, dan penerimaan luas sebagai standar internasional, membuatnya andal dan efisien untuk berbagai aplikasi enkripsi data.

## Bonus Video
Kami juga membuat video sebagai media dalam memahami aplikasi program ini lebih lanjut. Untuk melihat videonya bisa meng-klik [HapHipHop]
() berikut. 

<a name="features"></a>
## Features
Berikut merupakan fitur dari program kami:
* Visualisasi aplikasi yang membuat menarik
* Gambar sidik-jari yang ingin dicocokan dan sidik jari yang cocok
* Toggle switch untuk memilih algoritma KMP atau BM
* Tampilan biodata dari sidik-jari yang cocok

<a name="getting started"></a>
## Getting Started!

Berikut merupakan cara untuk build project atau menginstall program

1. Clone repo menggunakan command berikut

```
git clone https://github.com/zultopia/Tubes3_HapHipHop.git 
```

2. Setelah masuk ke folder program, jalankan perintah berikut untuk compile program utama!

Untuk windows:
```
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```
Untuk LINUX:
```
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
```
Untuk MACOS:
```
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true
```

## How-to-Run

Bagaimana cara menjalankan program? Gunakan command berikut pada folder program untuk menjalankan program utama

```
bin/hapHipHop
```
Kami juga menyediakan build yang sudah dapat Anda gunakan langsung. Pada folder bin, terdapat executable zip untuk MACOS dan Windows. Unzip file yang sesuai dengan sistem operasi kamu, dan jalankan file hapHipHop! Setelah itu, ikuti petunjuk pada program. 

     
<a name="tampilan"></a>
## Tampilan Program


<!-- LICENSE -->
## Licensing

The code in this project is licensed under MIT license.  
Code dalam projek ini berada di bawah lisensi MIT.

<br>
<h3 align="center"> TERIMA KASIH! </h3>

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[license-shield]: https://img.shields.io/github/license/othneildrew/Best-README-Template.svg?style=for-the-badge
[license-url]: https://github.com/zultopia/Tubes3_HapHipHop/blob/main/LICENSE