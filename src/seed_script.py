import psycopg2
import random
import os
from faker import Faker

db_config = {
    'dbname': 'Tubes3_HapHipHop',
    'user': 'postgres',
    'password': 'mockingjay07',
    'host': 'localhost',
    'port': '5432'
}

faker = Faker('id_ID')

def generate_alay_name(name):
    name = name.replace('a', '4').replace('i', '1').replace('e', '3').replace('o', '0').replace('u', 'v').replace("l", "1").replace("z", "2").replace("s", "5").replace("g", "6").replace("t", "7").replace("b", "8").replace("g", "9").replace("a", "@").replace("i", "!").replace("s", "$")
    name = ''.join(random.choice([k.upper(), k.lower()]) for k in name)
    return name

conn = psycopg2.connect(**db_config)
cursor = conn.cursor()

image_folder = 'SOCOFing/Real/'

def seed_data():
    try:
        sidik_jari_records = []
        biodata_records = []
        for i in range(600):
            nama = faker.name()
            for j in range(10):
                berkas_citra = os.path.join(image_folder, f'{i+1}__M_{["Left", "Right"][j//5]}_{["thumb", "index", "middle", "ring", "little"][j%5]}_finger.BMP')
                sidik_jari_records.append((berkas_citra, nama))

            NIK = faker.random_number(digits=16, fix_len=True)
            nama_alay = generate_alay_name(nama)
            tempat_lahir = faker.city()
            tanggal_lahir = faker.date_of_birth(minimum_age=18, maximum_age=60).strftime('%Y-%m-%d')
            jenis_kelamin = random.choice(['Laki-Laki', 'Perempuan'])
            golongan_darah = random.choice(['A', 'B', 'AB', 'O'])
            alamat = faker.address().replace('\n', ', ')
            agama = random.choice(['Islam', 'Kristen', 'Katholik', 'Hindu', 'Buddha', 'Konghucu'])
            status_perkawinan = random.choice(['Belum Menikah', 'Menikah', 'Cerai'])
            pekerjaan = faker.job()
            kewarganegaraan = faker.country()
            
            biodata_records.append((NIK, nama_alay, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan))

        insert_sidik_jari_query = '''
        INSERT INTO sidik_jari (berkas_citra, nama)
        VALUES (%s, %s)
        '''
        cursor.executemany(insert_sidik_jari_query, sidik_jari_records)

        insert_biodata_query = '''
        INSERT INTO biodata (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan)
        VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
        '''
        cursor.executemany(insert_biodata_query, biodata_records)

        conn.commit()
        print('Seeding data selesai.')
    except Exception as e:
        print(f'Error saat seeding data: {e}')
        conn.rollback()
    finally:
        cursor.close()
        conn.close()

seed_data()