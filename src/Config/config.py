import psycopg2

db_config = {
    'dbname': 'Tubes3_HapHipHop',
    'user': 'postgres',
    'password': '3663',
    'host': 'localhost',
    'port': '5432'
}

conn = psycopg2.connect(**db_config)
cursor = conn.cursor()

def delete_data():
    try:
        delete_biodata_query = '''
        WITH biodata_to_delete AS (
            SELECT ctid FROM biodata
            ORDER BY ctid
            LIMIT 1800
        )
        DELETE FROM biodata
        WHERE ctid IN (SELECT ctid FROM biodata_to_delete)
        '''
        cursor.execute(delete_biodata_query)

        delete_sidik_jari_query = '''
        WITH sidik_jari_to_delete AS (
            SELECT ctid FROM sidik_jari
            ORDER BY ctid
            LIMIT 12705
        )
        DELETE FROM sidik_jari
        WHERE ctid IN (SELECT ctid FROM sidik_jari_to_delete)
        '''
        cursor.execute(delete_sidik_jari_query)

        conn.commit()
        print('Data berhasil dihapus.')
    except Exception as e:
        print(f'Error saat menghapus data: {e}')
        conn.rollback()
    finally:
        cursor.close()
        conn.close()

delete_data()