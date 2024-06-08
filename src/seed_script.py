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

faker = Faker(['id_ID', 'en_US'])

def generate_alay_name(name, max_length=50):
    substitutions = {
        'a': ['4', '@'],
        'i': ['1', '!'],
        'e': ['3'],
        'o': ['0'],
        'u': ['v'],
        'l': ['1'],
        'z': ['2'],
        's': ['5', '$'],
        'g': ['6', '9'],
        't': ['7'],
        'b': ['8']
    }
    
    alay_name = ''.join(
        random.choice(substitutions.get(char.lower(), [char])).upper() if random.random() < 0.5 else 
        random.choice(substitutions.get(char.lower(), [char])).lower() 
        for char in name
    )
    
    if random.random() < 0.3:
        vowels = 'aeiouAEIOU'
        alay_name = ''.join([char for char in alay_name if char in vowels])

    alay_name = alay_name[:max_length]

    while not alay_name.strip():
        alay_name = ''.join(
            random.choice(substitutions.get(char.lower(), [char])).upper() if random.random() < 0.5 else 
            random.choice(substitutions.get(char.lower(), [char])).lower() 
            for char in name
        )
        alay_name = alay_name[:max_length]
    
    return alay_name

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