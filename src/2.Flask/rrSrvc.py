import json
from flask import Flask, request, jsonify

from pathlib import Path

app = Flask(__name__)

@app.route('/')
def index():
    return "Hello, World!"

@app.route('/fileData', methods=['POST'])
def find_data():
    data = json.loads(request.data)

    print(data['lang'])
    data['lang'] = 'default language'

    print(data['code_sample'])
    data['code_sample'] = """ 
        try {
            psRetrieveAll = con.prepareStatement("SELECT * FROM objects " + "WHERE (image_nm = ?) AND " + "(removal_dt is NULL) AND " + "(death_dt > ?) " + "ORDER BY insert_dt");
            psRetrieveAll.setString(1, image);
            psRetrieveAll.setTimestamp(2, new Timestamp(Calendar.getInstance().getTime().getTime()));
            ResultSet rs = psRetrieveAll.executeQuery();
            while (rs.next()) {
                DataInputStream dis = new DataInputStream(rs.getBinaryStream("data_str"));
                String retString = new String();
                try {
                    while (true) {
                        retString += dis.readChar();
                    }
                } catch (EOFException e) {
                }
                v.add(retString);
            }
            rs.close();
        } catch (Exception e) {
            dbFlag.freeBusyFlag();
            System.err.println(e);
            throw (new Exception(toString()));
        }
"""

    data['c1'] = getJson('file1.java')
    data['c2'] = getJson('file2.java')

    return jsonify(data)

def getJson(fileName):
  cur_path = str(Path().absolute())
  with open(cur_path + '/data/' + fileName, 'r') as f:
        data = f.read()
        records = json.dumps(data)
        return records

if __name__ == '__main__':
    app.run(debug=True)