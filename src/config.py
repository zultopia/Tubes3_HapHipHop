import psycopg2

db_config = {
    'dbname': 'Tubes3_HapHipHop',
    'user': 'postgres',
    'password': 'mockingjay07',
    'host': 'localhost',
    'port': '5432'
}

names_to_delete_biodata = [
    "Paul Thompson",
    "Sophia Gonzalez",
    "Katelyn Knight",
    "Melissa Pennington",
    "Brandy Smith",
    "Sandra Wilson",
    "Susan Roberts",
    "Maria Holland",
    "Eric Cox",
    "Robert Thompson"
]

names_to_delete_sidik_jari = [
    "Edward Ballard",
    "Brandon James",
    "Christian Garcia",
    "Alan Martinez",
    "Christopher Butler",
    "Mr. Jeffery Deleon",
    "Mrs. Kathleen Caldwell MD",
    "Amy Lee",
    "Gregory Perry",
    "Zoe Phillips"
]

try:
    conn = psycopg2.connect(**db_config)
    cursor = conn.cursor()
    
    delete_query_biodata = '''
    DELETE FROM biodata
    WHERE nama = ANY(%s)
    '''
    delete_query_sidik_jari = '''
    DELETE FROM sidik_jari
    WHERE nama = ANY(%s)
    '''

    cursor.execute(delete_query_biodata, (names_to_delete_biodata,))
    conn.commit()
    print(f'{cursor.rowcount} records deleted from biodata.')
    cursor.execute(delete_query_sidik_jari, (names_to_delete_sidik_jari,))
    conn.commit()
    print(f'{cursor.rowcount} records deleted from sidik_jari.')

except Exception as e:
    print(f'Error saat menghapus data: {e}')
    conn.rollback()
finally:
    cursor.close()
    conn.close()