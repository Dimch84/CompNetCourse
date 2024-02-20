package com.budee.crm.dao.core;

import java.util.List;
import org.hibernate.Query;
import org.hibernate.Transaction;
import com.budee.crm.pojo.accesscontrol.AcDataCustomer;
import com.budee.crm.pojo.accesscontrol.AcDataProject;
import com.budee.crm.pojo.crmdo.DoCustomer;
import com.budee.crm.pojo.crmdo.DoCustomerRelation;
import com.budee.crm.pojo.crmdo.DoProjectCustomer;
import com.budee.crm.pojo.crmdo.DoNote;
import com.budee.crm.pojo.crmdo.DoTodo;
import com.budee.crm.pojo.crmdo.DoAppointments;

public class DoCustomerDAO extends BaseHibernateDAO {

    public DoCustomer findCustomerBy(DoCustomer customer) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            StringBuffer hqlSB = new StringBuffer();
            hqlSB.append("from ");
            hqlSB.append(DoCustomer.class.getName());
            hqlSB.append(" where name = '" + customer.getName() + "'");
            hqlSB.append(" and mobilePhone = '" + customer.getMobilePhone() + "'");
            Query queryObject = getSession().createQuery(hqlSB.toString());
            List<DoCustomer> rtn = queryObject.list();
            tx.commit();
            return rtn.size() <= 0 ? null : rtn.get(0);
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public List getAllProjectCustomerPagination(Integer customerId, int offset, int limit, String sort, String order, String[] filters) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            StringBuffer hqlSB = new StringBuffer();
            hqlSB.append("from ");
            hqlSB.append(DoProjectCustomer.class.getName());
            hqlSB.append(" where ");
            hqlSB.append(" doCustomer.id = '" + customerId + "' and");
            hqlSB.append(" doProject.id in (");
            hqlSB.append(" select data.id from ");
            hqlSB.append(AcDataProject.class.getName());
            hqlSB.append(getWhereStatement(filters));
            hqlSB.append(") order by " + sort + " " + order);
            Query queryObject = getSession().createQuery(hqlSB.toString());
            queryObject.setFirstResult(offset);
            queryObject.setMaxResults(limit);
            List rtn = queryObject.list();
            tx.commit();
            return rtn;
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public int getProjectCustomerCount(Integer customerId, String[] filters) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            StringBuffer hqlSB = new StringBuffer();
            hqlSB.append("select count(*) from ");
            hqlSB.append(DoProjectCustomer.class.getName());
            hqlSB.append(" where ");
            hqlSB.append(" doCustomer.id = '" + customerId + "' and");
            hqlSB.append(" doProject.id in (");
            hqlSB.append(" select data.id from ");
            hqlSB.append(AcDataProject.class.getName());
            hqlSB.append(getWhereStatement(filters));
            hqlSB.append(")");
            Query queryObject = getSession().createQuery(hqlSB.toString());
            Integer count = ((Integer) queryObject.iterate().next()).intValue();
            tx.commit();
            return count;
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public DoProjectCustomer getProjectCustomer(Integer customerId, Integer projectId) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            String queryString = "from " + DoProjectCustomer.class.getName() + " where doProject.id = '" + projectId + "' and doCustomer.id = '" + customerId + "'";
            Query queryObject = getSession().createQuery(queryString);
            List<DoProjectCustomer> rtn = queryObject.list();
            tx.commit();
            return rtn.size() <= 0 ? null : rtn.get(0);
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public List<DoNote> getAllCustomerNotePagination(int customerId, int offset, int limit, String sort, String order, String[] filters) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            StringBuffer hqlSB = new StringBuffer();
            hqlSB.append("from ");
            hqlSB.append(DoNote.class.getName());
            hqlSB.append(getWhereStatement(filters));
            if (filters != null && filters.length != 0) {
                hqlSB.append(" and ");
            } else {
                hqlSB.append(" where ");
            }
            hqlSB.append(" doCustomer.id = '" + customerId + "' or (");
            hqlSB.append(getWhereClause(filters));
            hqlSB.append(" doCustomer.id ='" + customerId + "' and noteType='文档附件(共享)')");
            hqlSB.append(" order by " + sort + " " + order);
            Query queryObject = getSession().createQuery(hqlSB.toString());
            queryObject.setFirstResult(offset);
            queryObject.setMaxResults(limit);
            List<DoNote> rtn = queryObject.list();
            tx.commit();
            return rtn;
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public Integer getAllCustomerNoteCount(int customerId, String[] filters) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            StringBuffer hqlSB = new StringBuffer();
            hqlSB.append("select count(*) from ");
            hqlSB.append(DoNote.class.getName());
            hqlSB.append(getWhereStatement(filters));
            if (filters != null && filters.length != 0) {
                hqlSB.append(" and ");
            } else {
                hqlSB.append(" where ");
            }
            hqlSB.append(" doCustomer.id = '" + customerId + "'");
            Query queryObject = getSession().createQuery(hqlSB.toString());
            Integer count = ((Integer) queryObject.iterate().next()).intValue();
            tx.commit();
            return count;
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public List<DoTodo> getAllCustomerTodoPagination(int customerId, int offset, int limit, String sort, String order, String[] filters) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            StringBuffer hqlSB = new StringBuffer();
            hqlSB.append("from ");
            hqlSB.append(DoTodo.class.getName());
            hqlSB.append(getWhereStatement(filters));
            if (filters != null && filters.length != 0) {
                hqlSB.append(" and ");
            } else {
                hqlSB.append(" where ");
            }
            hqlSB.append(" doCustomer.id = '" + customerId + "' order by " + sort + " " + order);
            Query queryObject = getSession().createQuery(hqlSB.toString());
            queryObject.setFirstResult(offset);
            queryObject.setMaxResults(limit);
            List<DoTodo> rtn = queryObject.list();
            tx.commit();
            return rtn;
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public Integer getAllCustomerTodoCount(int customerId, String[] filters) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            StringBuffer hqlSB = new StringBuffer();
            hqlSB.append("select count(*) from ");
            hqlSB.append(DoTodo.class.getName());
            hqlSB.append(getWhereStatement(filters));
            if (filters != null && filters.length != 0) {
                hqlSB.append(" and ");
            } else {
                hqlSB.append(" where ");
            }
            hqlSB.append(" doCustomer.id = '" + customerId + "'");
            Query queryObject = getSession().createQuery(hqlSB.toString());
            Integer count = ((Integer) queryObject.iterate().next()).intValue();
            tx.commit();
            return count;
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public List<DoAppointments> getAllCustomerAppointmentPagination(int customerId, int offset, int limit, String sort, String order, String[] filters) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            StringBuffer hqlSB = new StringBuffer();
            hqlSB.append("from ");
            hqlSB.append(DoAppointments.class.getName());
            hqlSB.append(getWhereStatement(filters));
            if (filters != null && filters.length != 0) {
                hqlSB.append(" and ");
            } else {
                hqlSB.append(" where ");
            }
            hqlSB.append(" doCustomer.id = '" + customerId + "' order by " + sort + " " + order);
            Query queryObject = getSession().createQuery(hqlSB.toString());
            queryObject.setFirstResult(offset);
            queryObject.setMaxResults(limit);
            List<DoAppointments> rtn = queryObject.list();
            tx.commit();
            return rtn;
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public Integer getAllCustomerAppointmentCount(int customerId, String[] filters) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            StringBuffer hqlSB = new StringBuffer();
            hqlSB.append("select count(*) from ");
            hqlSB.append(DoAppointments.class.getName());
            hqlSB.append(getWhereStatement(filters));
            if (filters != null && filters.length != 0) {
                hqlSB.append(" and ");
            } else {
                hqlSB.append(" where ");
            }
            hqlSB.append(" doCustomer.id = '" + customerId + "'");
            Query queryObject = getSession().createQuery(hqlSB.toString());
            Integer count = ((Integer) queryObject.iterate().next()).intValue();
            tx.commit();
            return count;
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public AcDataCustomer getAcDataCustomer(Integer customerId, Integer userId) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            String queryString = "from " + AcDataCustomer.class.getName() + " where data.id = '" + customerId + "' and viewer.id = '" + userId + "'";
            Query queryObject = getSession().createQuery(queryString);
            List<AcDataCustomer> rtn = queryObject.list();
            tx.commit();
            return rtn.size() <= 0 ? null : rtn.get(0);
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public List<AcDataCustomer> getAllAcDataCustomerPagination(Integer customerId, int offset, int limit, String sort, String dir, String[] filters) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            StringBuffer hqlSB = new StringBuffer();
            hqlSB.append("from ");
            hqlSB.append(AcDataCustomer.class.getName());
            hqlSB.append(getWhereStatement(filters));
            if (filters != null && filters.length != 0) {
                hqlSB.append(" and ");
            } else {
                hqlSB.append(" where ");
            }
            hqlSB.append(" data.id = '" + customerId + "' order by " + sort + " " + dir);
            Query queryObject = getSession().createQuery(hqlSB.toString());
            queryObject.setFirstResult(offset);
            queryObject.setMaxResults(limit);
            List rtn = queryObject.list();
            tx.commit();
            return rtn;
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public Integer getAcDataCustomerCount(Integer customerId, String[] filters) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            StringBuffer hqlSB = new StringBuffer();
            hqlSB.append("select count(*) from ");
            hqlSB.append(AcDataCustomer.class.getName());
            hqlSB.append(getWhereStatement(filters));
            if (filters != null && filters.length != 0) {
                hqlSB.append(" and ");
            } else {
                hqlSB.append(" where ");
            }
            hqlSB.append(" data.id = '" + customerId + "'");
            Query queryObject = getSession().createQuery(hqlSB.toString());
            Integer count = ((Integer) queryObject.iterate().next()).intValue();
            tx.commit();
            return count;
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public void deleteAcDataCustomer(Integer customerId, String[] userIds) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            String delStr = null;
            for (int i = 0; i < userIds.length; i++) {
                if (userIds[i] == null) {
                    continue;
                }
                delStr = "delete from " + AcDataCustomer.class.getName() + " where data.id = '" + customerId + "' and viewer.id = '" + userIds[i] + "'";
                Query queryObject = getSession().createQuery(delStr);
                queryObject.executeUpdate();
            }
            tx.commit();
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public Object getCustomerRelation(Integer id, Integer customerId) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            String queryString = "from " + DoCustomerRelation.class.getName() + " where (doCustomerByDoCustomerId1.id = '" + id + "' and doCustomerByDoCustomerId2.id = '" + customerId + "') or (doCustomerByDoCustomerId2.id = '" + id + "' and doCustomerByDoCustomerId1.id = '" + customerId + "') ";
            Query queryObject = getSession().createQuery(queryString);
            List<DoCustomerRelation> rtn = queryObject.list();
            tx.commit();
            return rtn.size() <= 0 ? null : rtn.get(0);
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public void deleteCustomerRelation(Integer id, String[] customerIds) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            String delStr = null;
            for (int i = 0; i < customerIds.length; i++) {
                delStr = "delete from " + DoCustomerRelation.class.getName() + " where (doCustomerByDoCustomerId1.id = '" + id + "' and doCustomerByDoCustomerId2.id = '" + customerIds[i] + "') or (doCustomerByDoCustomerId2.id = '" + id + "' and doCustomerByDoCustomerId1.id = '" + customerIds[i] + "') ";
                Query queryObject = getSession().createQuery(delStr);
                queryObject.executeUpdate();
            }
            tx.commit();
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public List getAllCustomerRelationPagination(Integer customerId, int offset, int limit, String sort, String order, String[] filters) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            StringBuffer hqlSB = new StringBuffer();
            hqlSB.append("from ");
            hqlSB.append(DoCustomerRelation.class.getName());
            hqlSB.append(getWhereStatement(filters));
            if (filters != null && filters.length != 0) {
                hqlSB.append(" and ");
            } else {
                hqlSB.append(" where ");
            }
            hqlSB.append(" (doCustomerByDoCustomerId1.id = '" + customerId + "' or doCustomerByDoCustomerId2.id = '" + customerId + "')" + " order by " + sort + " " + order);
            Query queryObject = getSession().createQuery(hqlSB.toString());
            queryObject.setFirstResult(offset);
            queryObject.setMaxResults(limit);
            List rtn = queryObject.list();
            tx.commit();
            return rtn;
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }

    public Integer getCustomerRelationCount(Integer customerId, String[] filters) throws Exception {
        Transaction tx = null;
        try {
            tx = getSession().beginTransaction();
            StringBuffer hqlSB = new StringBuffer();
            hqlSB.append("select count(*) from ");
            hqlSB.append(DoCustomerRelation.class.getName());
            hqlSB.append(getWhereStatement(filters));
            if (filters != null && filters.length != 0) {
                hqlSB.append(" and ");
            } else {
                hqlSB.append(" where ");
            }
            hqlSB.append(" (doCustomerByDoCustomerId1.id = '" + customerId + "' or doCustomerByDoCustomerId2.id = '" + customerId + "')");
            Query queryObject = getSession().createQuery(hqlSB.toString());
            Integer count = ((Integer) queryObject.iterate().next()).intValue();
            ;
            tx.commit();
            return count;
        } catch (Exception e) {
            tx.rollback();
            e.printStackTrace();
            throw e;
        }
    }
}
