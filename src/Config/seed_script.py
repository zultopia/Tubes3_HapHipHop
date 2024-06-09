import datetime
import psycopg2
import random
import os
from faker import Faker
import math
import base64

db_config = {
    'dbname': 'Tubes3_HapHipHop',
    'user': 'postgres',
    'password': '3663',
    'host': 'localhost',
    'port': '5432'
}

faker = Faker(['id_ID', 'en_US'])

sBox = [
    0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5,
        0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,
        0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0,
        0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,
        0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc,
        0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,
        0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a,
        0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,
        0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0,
        0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,
        0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b,
        0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
        0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85,
        0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,
        0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5,
        0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,
        0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17,
        0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,
        0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88,
        0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,
        0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c,
        0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,
        0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9,
        0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,
        0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6,
        0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,
        0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e,
        0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,
        0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94,
        0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
        0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68,
        0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16
]

invSBox = [
    0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38,
        0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb,
        0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87,
        0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb,
        0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d,
        0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e,
        0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2,
        0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25,
        0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16,
        0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92,
        0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda,
        0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84,
        0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a,
        0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06,
        0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02,
        0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b,
        0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea,
        0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73,
        0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85,
        0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e,
        0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89,
        0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b,
        0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20,
        0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4,
        0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31,
        0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f,
        0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d,
        0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef,
        0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0,
        0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61,
        0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26,
        0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d
]

def SubBytes(state):
    for i in range(len(state)):
        state[i] = sBox[state[i]]
    return state

def InvSubBytes(state):
    for i in range(len(state)):
        state[i] = invSBox[state[i]]
    return state

def AddRoundKey(state, roundKey):
    for i in range(len(state)):
        state[i] ^= roundKey[i]
    return state

def PadRight(input_bytes, length):
    padded = bytearray(length)
    padded[:len(input_bytes)] = input_bytes
    return padded

def Encrypt(plain_text, key):
    key_bytes = key.encode('utf-8').ljust(16, b'\x00')
    plain_bytes = plain_text.encode('utf-8')
    number_of_blocks = math.ceil(len(plain_bytes) / 16)
    cipher_bytes = bytearray(number_of_blocks * 16)

    for block_index in range(number_of_blocks):
        block = bytearray(16)
        bytes_to_copy = min(16, len(plain_bytes) - block_index * 16)
        block[:bytes_to_copy] = plain_bytes[block_index * 16 : block_index * 16 + bytes_to_copy]

        block = AddRoundKey(block, key_bytes)
        block = SubBytes(block)
        block = AddRoundKey(block, key_bytes)

        cipher_bytes[block_index * 16 : block_index * 16 + 16] = block

    return base64.b64encode(cipher_bytes).decode('utf-8')

def Decrypt(cipher_text, key):
    key_bytes = key.encode('utf-8').ljust(16, b'\x00')
    cipher_bytes = base64.b64decode(cipher_text)
    number_of_blocks = len(cipher_bytes) // 16
    plain_bytes = bytearray(len(cipher_bytes))

    for block_index in range(number_of_blocks):
        block = bytearray(16)
        block[:] = cipher_bytes[block_index * 16 : block_index * 16 + 16]

        block = AddRoundKey(block, key_bytes)
        block = InvSubBytes(block)
        block = AddRoundKey(block, key_bytes)

        plain_bytes[block_index * 16 : block_index * 16 + 16] = block

    return plain_bytes.decode('utf-8').rstrip('\x00')

def EncryptDate(date, key):
    date_string = date.strftime('%Y-%m-%d')
    return Encrypt(date_string, key)

def DecryptDate(cipher_text, key):
    date_string = Decrypt(cipher_text, key)
    return datetime.strptime(date_string, '%Y-%m-%d')

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

    def substitute_char(char):
        return random.choice(substitutions.get(char.lower(), [char]))

    alay_name = ''.join(
        substitute_char(char).upper() if random.random() < 0.5 else substitute_char(char).lower()
        for char in name
    )
    
    def shorten_vowels(text):
        vowels = 'aeiouAEIOU'
        shortened = []
        for i, char in enumerate(text):
            if char.lower() in vowels and random.random() < 0.5:
                continue  
            shortened.append(char)
        return ''.join(shortened)
    
    alay_name = shorten_vowels(alay_name)
    
    alay_name = alay_name[:max_length]

    while not alay_name.strip():
        alay_name = ''.join(
            substitute_char(char).upper() if random.random() < 0.5 else substitute_char(char).lower()
            for char in name
        )
        alay_name = shorten_vowels(alay_name)
        alay_name = alay_name[:max_length]
    
    return alay_name

conn = psycopg2.connect(**db_config)
cursor = conn.cursor()

image_folder = '../../test/SOCOFing/Real/'

def seed_data():
    try:
        sidik_jari_records = []
        biodata_records = []
        
        # Get list of files in the image folder
        image_files = os.listdir(image_folder)
        
        # Dictionary to store people by their image number prefix
        people = {}

        for image_file in image_files:
            if image_file.endswith('.BMP'):
                # Extract the number prefix from the file name
                number_prefix = image_file.split('__')[0]
                
                # Check if this prefix already has a person assigned
                if number_prefix not in people:
                    # Create a new person for this prefix
                    nama = faker.name()
                    NIK = faker.random_number(digits=16, fix_len=True)
                    nama_alay = generate_alay_name(nama)
                    tempat_lahir = faker.city()
                    tanggal_lahir = faker.date_of_birth(minimum_age=18, maximum_age=60)
                    jenis_kelamin = random.choice(['Laki-Laki', 'Perempuan'])
                    golongan_darah = random.choice(['A', 'B', 'AB', 'O'])
                    alamat = faker.address().replace('\n', ', ')
                    agama = random.choice(['Islam', 'Kristen', 'Katholik', 'Hindu', 'Buddha', 'Konghucu'])
                    status_perkawinan = random.choice(['Belum Menikah', 'Menikah', 'Cerai'])
                    pekerjaan = faker.job()
                    kewarganegaraan = faker.country()

                    key = "abcdefghijklmnop"
                    NIK_enc = Encrypt(str(NIK), key)
                    Nama_enc = Encrypt(nama_alay, key)
                    Tempat_Lahir_enc = Encrypt(tempat_lahir, key)
                    Tanggal_Lahir_enc = EncryptDate(tanggal_lahir, key)
                    Goldar_enc = Encrypt(golongan_darah, key)
                    Alamat_enc = Encrypt(alamat, key)
                    Agama_enc = Encrypt(agama, key)
                    Pekerjaan_enc = Encrypt(pekerjaan, key)
                    Kewarganegaraan_enc = Encrypt(kewarganegaraan, key)

                    biodata_records.append((NIK_enc, Nama_enc, Tempat_Lahir_enc, Tanggal_Lahir_enc, jenis_kelamin, Goldar_enc, Alamat_enc, Agama_enc, status_perkawinan, Pekerjaan_enc, Kewarganegaraan_enc))

                    # Store the person's data in the dictionary
                    people[number_prefix] = nama

                # Assign the file to the person
                nama = people[number_prefix]
                berkas_citra = os.path.join("SOCOFing\Real", image_file)
                sidik_jari_records.append((berkas_citra, nama))

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