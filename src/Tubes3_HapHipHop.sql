PGDMP  !    
        	        |            Tubes3_HapHipHop    16.3    16.3                0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false                       0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false                       0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false                       1262    16398    Tubes3_HapHipHop    DATABASE     t   CREATE DATABASE "Tubes3_HapHipHop" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'C';
 "   DROP DATABASE "Tubes3_HapHipHop";
                postgres    false            I           1247    16400    jenis_kelamin_enum    TYPE     T   CREATE TYPE public.jenis_kelamin_enum AS ENUM (
    'Laki-Laki',
    'Perempuan'
);
 %   DROP TYPE public.jenis_kelamin_enum;
       public          postgres    false            L           1247    16406    status_perkawinan_enum    TYPE     g   CREATE TYPE public.status_perkawinan_enum AS ENUM (
    'Belum Menikah',
    'Menikah',
    'Cerai'
);
 )   DROP TYPE public.status_perkawinan_enum;
       public          postgres    false            �            1259    16413    biodata    TABLE     �  CREATE TABLE public.biodata (
    nik character varying(200) NOT NULL,
    nama character varying(255) DEFAULT NULL::character varying,
    tempat_lahir character varying(255) DEFAULT NULL::character varying,
    tanggal_lahir text,
    jenis_kelamin public.jenis_kelamin_enum,
    golongan_darah character varying(100) DEFAULT NULL::character varying,
    alamat character varying(1000) DEFAULT NULL::character varying,
    agama character varying(100) DEFAULT NULL::character varying,
    status_perkawinan public.status_perkawinan_enum,
    pekerjaan character varying(255) DEFAULT NULL::character varying,
    kewarganegaraan character varying(255) DEFAULT NULL::character varying
);
    DROP TABLE public.biodata;
       public         heap    postgres    false    841    844            �            1259    16427 
   sidik_jari    TABLE     {   CREATE TABLE public.sidik_jari (
    berkas_citra text,
    nama character varying(100) DEFAULT NULL::character varying
);
    DROP TABLE public.sidik_jari;
       public         heap    postgres    false                      0    16413    biodata 
   TABLE DATA           �   COPY public.biodata (nik, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan) FROM stdin;
    public          postgres    false    215                    0    16427 
   sidik_jari 
   TABLE DATA           8   COPY public.sidik_jari (berkas_citra, nama) FROM stdin;
    public          postgres    false    216   2       �           2606    16558    biodata biodata_pkey 
   CONSTRAINT     S   ALTER TABLE ONLY public.biodata
    ADD CONSTRAINT biodata_pkey PRIMARY KEY (nik);
 >   ALTER TABLE ONLY public.biodata DROP CONSTRAINT biodata_pkey;
       public            postgres    false    215                  x������ � �            x������ � �     