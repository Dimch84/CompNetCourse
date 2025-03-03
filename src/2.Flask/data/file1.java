package edu.idp.server;

import ex.Server;
import edu.idp.server.database.Database;
import edu.idp.shared.StoredObject;
import edu.idp.shared.net.BusyFlag;
import edu.idp.shared.net.NetTransferObject;
import edu.idp.shared.utils.Convert;
import java.io.BufferedReader;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.EOFException;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.Socket;
import java.sql.DatabaseMetaData;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.SQLWarning;
import java.sql.Statement;
import java.sql.Timestamp;
import java.util.Calendar;
import java.util.Enumeration;
import java.util.LinkedList;
import java.util.Vector;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;

/**
 * My own little server service
 *
 * @author Kristopher T Babic
 */
public class InterDrawService implements Server.Service {

    private BusyFlag dbFlag = new BusyFlag();

    private BusyFlag idFlag = new BusyFlag();

    private char[] id;

    private LinkedList saved = null;

    private java.sql.Connection con;

    private final int MAX_DIFF = 5000;

    private PreparedStatement psRetrieveSome;

    private PreparedStatement psRetrieveAll;

    private PreparedStatement psStore;

    private PreparedStatement psRemove;

    public InterDrawService() {
        if (!dbConnect()) System.exit(1);
    }

    private synchronized boolean dbConnect() {
        try {
            if (con != null && !con.isClosed()) return true;
        } catch (Exception e) {
        }
        try {
            Class.forName(Database.DRIVER);
            con = DriverManager.getConnection(Database.URL, Database.USER, Database.PASSWORD);
            checkForWarning(con.getWarnings());
            DatabaseMetaData dma = con.getMetaData();
            psStore = con.prepareStatement("INSERT INTO objects " + "(insert_dt, user_id, object_id, image_nm, death_dt, data_str) " + "VALUES (?,?,?,?,?,?)");
            psRemove = con.prepareStatement("UPDATE objects " + "SET removal_dt = ? " + "WHERE (user_id = ? ) AND " + "(image_nm = ? ) AND " + "(object_id = ? )");
            System.out.println("\nConnected to " + dma.getURL());
            System.out.println("Driver       " + dma.getDriverName());
            System.out.println("Version      " + dma.getDriverVersion());
            System.out.println("");
        } catch (SQLException ex) {
            System.out.println("\n*** SQLException caught ***\n");
            while (ex != null) {
                System.out.println("SQLState: " + ex.getSQLState());
                System.out.println("Message:  " + ex.getMessage());
                System.out.println("Vendor:   " + ex.getErrorCode());
                ex = ex.getNextException();
                System.out.println("");
                if (con != null) {
                    try {
                        con.close();
                    } catch (Exception e) {
                    }
                }
                con = null;
                return false;
            }
        } catch (java.lang.Exception ex) {
            ex.printStackTrace();
            if (con != null) {
                try {
                    con.close();
                } catch (Exception e) {
                }
            }
            con = null;
            return false;
        }
        return true;
    }

    private boolean checkForWarning(SQLWarning warn) throws SQLException {
        boolean rc = false;
        if (warn != null) {
            System.out.println("\n *** Warning ***\n");
            rc = true;
            while (warn != null) {
                System.out.println("SQLState: " + warn.getSQLState());
                System.out.println("Message:  " + warn.getMessage());
                System.out.println("Vendor:   " + warn.getErrorCode());
                System.out.println("");
                warn = warn.getNextWarning();
            }
        }
        return rc;
    }

    private String getNewId() {
        idFlag.getBusyFlag();
        if (id == null) {
            id = new char[NetTransferObject.MAX_LEN];
            for (int i = 0; i < NetTransferObject.MAX_LEN; i++) id[i] = (char) 1;
        }
        String newId = new String(id);
        id[NetTransferObject.MAX_LEN - 1]++;
        for (int i = (NetTransferObject.MAX_LEN - 1); i >= 0; i--) {
            if (id[i] == Character.MAX_VALUE) {
                id[i] = 1;
                if (i != 1) id[i - 1]++;
                for (int j = i; j < NetTransferObject.MAX_LEN; j++) id[j] = 1;
            } else break;
        }
        idFlag.freeBusyFlag();
        return newId;
    }

    private NetTransferObject getUncompressedInput(InputStream i) throws Exception {
        NetTransferObject input = new NetTransferObject();
        boolean done = false;
        while (!done) {
            try {
                input.readData(i);
                done = true;
            } catch (java.io.InterruptedIOException err) {
            }
        }
        return input;
    }

    private String getCompressedInput(InputStream i) {
        try {
            ZipInputStream in = new ZipInputStream(i);
            ZipEntry z;
            while ((z = in.getNextEntry()) != null) {
                if (z.getName().equals("done")) {
                    System.out.println("i found the end");
                    System.out.flush();
                    return "e";
                }
                System.out.println(z.getName() + ":");
                System.out.flush();
                BufferedReader zin = new BufferedReader(new InputStreamReader(in));
                String s;
                while ((s = zin.readLine()) != null) System.out.println(s);
                System.out.flush();
                in.closeEntry();
            }
            in.close();
        } catch (IOException e) {
            System.err.println("ioexeption");
            return null;
        }
        return new String("lkj");
    }

    public void serve(Socket s, InputStream i, OutputStream o) throws IOException {
        boolean done = false;
        try {
            DataOutputStream out = new DataOutputStream(o);
            dbConnect();
            while (!done) {
                NetTransferObject input = getUncompressedInput(i);
                dbConnect();
                char ch = input.getCommand();
                String toClient;
                switch(ch) {
                    case NetTransferObject.CMD_GET_UID:
                        System.out.flush();
                        input.reset();
                        input.setCommand(NetTransferObject.CMD_DATA);
                        input.addData(getNewId());
                        input.sendData(o);
                        break;
                    case NetTransferObject.CMD_GET_ALL:
                        try {
                            Vector retVector = dbRetrieve(input.getImageName());
                            input.reset();
                            input.setCommand(NetTransferObject.CMD_DATA);
                            for (int j = 0; j < retVector.size(); j++) input.addData((String) retVector.elementAt(j));
                        } catch (Exception e) {
                            input.reset();
                            input.setCommand(NetTransferObject.ERROR);
                            input.addData(e.toString());
                        }
                        input.sendData(o);
                        break;
                    case NetTransferObject.CMD_GET_TIME:
                        Enumeration enmr = input.getData();
                        String tData = null;
                        if (enmr.hasMoreElements()) tData = (String) enmr.nextElement(); else tData = Convert.fromLong(0l);
                        long cTime = Convert.toLong(tData);
                        long sTime = Calendar.getInstance().getTime().getTime();
                        long diff = sTime - cTime;
                        if (Math.abs(diff) < 2000) diff = 0;
                        input.reset();
                        input.setCommand(NetTransferObject.CMD_DATA);
                        input.addData(Convert.fromLong(diff));
                        input.sendData(o);
                        break;
                    case NetTransferObject.CMD_GET_NEW:
                        long newTime = 0l;
                        enmr = input.getData();
                        if (enmr.hasMoreElements()) newTime = Convert.toLong((String) enmr.nextElement());
                        Vector retVectorAll = null;
                        try {
                            retVectorAll = dbRetrieve(input.getUID(), newTime, input.getImageName());
                            input.reset();
                            for (int j = 0; j < retVectorAll.size(); j++) input.addData((String) retVectorAll.elementAt(j));
                            Vector retRemoved = dbRetrieveRemoved(input.getUID(), newTime, input.getImageName());
                            for (int j = 0; j < retRemoved.size(); j++) input.addRemove((String) retRemoved.elementAt(j));
                            input.setCommand(NetTransferObject.CMD_DATA);
                        } catch (Exception e) {
                            input.reset();
                            input.setCommand(NetTransferObject.ERROR);
                            input.addData(e.toString());
                        }
                        input.sendData(o);
                        break;
                    case NetTransferObject.CMD_ADD:
                        try {
                            dbSetRemoved(input.getRemovedData(), input.getUID(), input.getImageName());
                            dbStore(input.getData(), input.getUID(), input.getImageName());
                            input.reset();
                            input.setCommand(NetTransferObject.OK);
                        } catch (Exception e) {
                            input.reset();
                            input.setCommand(NetTransferObject.ERROR);
                            input.addData(e.toString());
                        }
                        input.sendData(o);
                        break;
                    case NetTransferObject.CMD_CREATE:
                        break;
                    case NetTransferObject.CMD_DATA:
                        break;
                    case NetTransferObject.CMD_REMOVE:
                        try {
                            dbSetRemoved(input.getRemovedData(), input.getUID(), input.getImageName());
                            input.reset();
                            input.setCommand(NetTransferObject.OK);
                        } catch (Exception e) {
                            input.reset();
                            input.setCommand(NetTransferObject.ERROR);
                        }
                        input.sendData(o);
                        break;
                    case NetTransferObject.CMD_GET_IMAGES:
                        try {
                            Vector retImages = dbRetrieveImages();
                            input.reset();
                            for (int j = 0; j < retImages.size(); j++) input.addData((String) retImages.elementAt(j));
                            input.setCommand(NetTransferObject.CMD_DATA);
                        } catch (Exception e) {
                            input.reset();
                            input.setCommand(NetTransferObject.ERROR);
                            input.addData(e.toString());
                        }
                        input.sendData(o);
                        break;
                    default:
                        System.out.println("Received invalid command: " + ((int) ch));
                        done = true;
                        break;
                }
            }
        } catch (Exception e) {
            System.out.println("error: ");
            System.out.flush();
            e.printStackTrace();
        }
        System.out.flush();
        try {
            i.close();
            o.close();
        } catch (Exception e) {
        }
    }

    private Vector dbRetrieveImages() throws Exception {
        dbFlag.getBusyFlag();
        Vector v = new Vector();
        Statement psRetrieveImages = null;
        try {
            psRetrieveImages = con.createStatement();
            ResultSet rs = psRetrieveImages.executeQuery("SELECT image_nm FROM objects " + "GROUP BY image_nm " + "ORDER BY image_nm");
            while (rs.next()) {
                v.add(rs.getString("image_nm"));
            }
        } catch (Exception e) {
            dbFlag.freeBusyFlag();
            e.printStackTrace();
            throw (new Exception(e.toString()));
        }
        try {
            psRetrieveImages.close();
        } catch (Exception e) {
        }
        dbFlag.freeBusyFlag();
        return v;
    }

    private Vector dbRetrieveRemoved(String uid, long time, String image) throws Exception {
        dbFlag.getBusyFlag();
        Vector v = new Vector();
        try {
            psRetrieveSome = con.prepareStatement("SELECT * FROM objects " + "WHERE (NOT (user_id = ?)) AND " + "(image_nm = ?) AND " + "(NOT (removal_dt is NULL)) AND " + "(removal_dt > ?) " + "ORDER BY insert_dt");
            psRetrieveSome.setString(1, uid);
            psRetrieveSome.setString(2, image);
            psRetrieveSome.setTimestamp(3, new Timestamp(time));
            ResultSet rs = psRetrieveSome.executeQuery();
            while (rs.next()) {
                v.add(rs.getString("object_id"));
            }
        } catch (Exception e) {
            dbFlag.freeBusyFlag();
            e.printStackTrace();
            throw (new Exception(e.toString()));
        }
        try {
            psRetrieveSome.close();
        } catch (Exception e) {
        }
        dbFlag.freeBusyFlag();
        return v;
    }

    private void dbSetRemoved(Enumeration eData, String uid, String image) throws Exception {
        if (eData == null) {
            return;
        }
        dbFlag.getBusyFlag();
        try {
            con.setAutoCommit(false);
            int i = 500;
            while (eData.hasMoreElements()) {
                String object = (String) eData.nextElement();
                psRemove.clearParameters();
                Timestamp st = new Timestamp(Calendar.getInstance().getTime().getTime());
                psRemove.setTimestamp(1, st);
                psRemove.setString(2, uid);
                psRemove.setString(3, image);
                psRemove.setString(4, object);
                int rows = psRemove.executeUpdate();
            }
            con.commit();
        } catch (SQLException e) {
            while (e != null) {
                System.err.println("state:" + e.getSQLState() + "\ncode:" + e.getErrorCode() + "\nstring:" + e.toString() + "\nstack:");
                e.printStackTrace();
                e = e.getNextException();
            }
            try {
                con.rollback();
            } catch (Exception ex) {
                con = null;
            }
            dbFlag.freeBusyFlag();
            throw (new Exception(e.toString()));
        } catch (Exception e) {
            dbFlag.freeBusyFlag();
            e.printStackTrace();
            try {
                con.rollback();
            } catch (Exception ex) {
                con = null;
            }
            throw (new Exception(e.toString()));
        }
        try {
            con.setAutoCommit(true);
        } catch (Exception e) {
        }
        dbFlag.freeBusyFlag();
    }

    private void dbStore(Enumeration eData, String uid, String image) throws Exception {
        if (eData == null) return;
        dbFlag.getBusyFlag();
        try {
            con.setAutoCommit(false);
            int i = 500;
            while (eData.hasMoreElements()) {
                String dString = (String) eData.nextElement();
                long deathDate = StoredObject.findDeath(dString);
                String id = StoredObject.findID(dString);
                psStore.clearParameters();
                Timestamp st = new Timestamp(Calendar.getInstance().getTime().getTime());
                psStore.setTimestamp(1, st);
                psStore.setString(2, uid);
                psStore.setString(3, id);
                psStore.setString(4, image);
                psStore.setTimestamp(5, new Timestamp(deathDate));
                ByteArrayOutputStream baos = new ByteArrayOutputStream();
                DataOutputStream dos = new DataOutputStream(baos);
                dos.writeChars(dString);
                ByteArrayInputStream bais = new ByteArrayInputStream(baos.toByteArray());
                psStore.setBinaryStream(6, bais, bais.available());
                int rows = psStore.executeUpdate();
            }
            con.commit();
        } catch (SQLException e) {
            while (e != null) {
                System.err.println("state:" + e.getSQLState() + "\ncode:" + e.getErrorCode() + "\nstring:" + e.toString() + "\nstack:");
                e.printStackTrace();
                e = e.getNextException();
            }
            try {
                con.rollback();
            } catch (Exception ex) {
                con = null;
            }
            dbFlag.freeBusyFlag();
            throw (e);
        } catch (Exception e) {
            dbFlag.freeBusyFlag();
            e.printStackTrace();
            try {
                con.rollback();
            } catch (Exception ex) {
                con = null;
            }
            throw (new Exception(e.toString()));
        }
        try {
            con.setAutoCommit(true);
        } catch (Exception e) {
        }
        dbFlag.freeBusyFlag();
    }

    private Vector dbRetrieve(String uid, long time, String image) throws Exception {
        dbFlag.getBusyFlag();
        Vector v = new Vector();
        try {
            psRetrieveSome = con.prepareStatement("SELECT * FROM objects " + "WHERE (NOT (user_id = ?)) AND " + "(image_nm = ?) AND " + "(insert_dt > ?) AND " + "(removal_dt is NULL) AND " + "(death_dt > ?) " + "ORDER BY insert_dt");
            psRetrieveSome.setString(1, uid);
            psRetrieveSome.setString(2, image);
            psRetrieveSome.setTimestamp(3, new Timestamp(time));
            psRetrieveSome.setTimestamp(4, new Timestamp(Calendar.getInstance().getTime().getTime()));
            ResultSet rs = psRetrieveSome.executeQuery();
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
        } catch (Exception e) {
            dbFlag.freeBusyFlag();
            e.printStackTrace();
            throw (new Exception(e.toString()));
        }
        try {
            psRetrieveSome.close();
        } catch (Exception e) {
        }
        dbFlag.freeBusyFlag();
        return v;
    }

    private Vector dbRetrieve(String image) throws Exception {
        dbFlag.getBusyFlag();
        Vector v = new Vector();
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
        try {
            psRetrieveAll.clearParameters();
            psRetrieveAll.close();
        } catch (Exception e) {
        }
        dbFlag.freeBusyFlag();
        return v;
    }
}
