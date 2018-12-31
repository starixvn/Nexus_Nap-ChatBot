import React, { Component } from 'react';
import { connect } from 'react-redux';
import NCBBreadcrum from '../../common/breadcrumbs';
import NewNotificationType from "./NewNotificationType";
import ConfirmPopup from "../../common/ConfirmPopup";
import { bindActionCreators } from 'redux';
import * as NotificationTypeList  from '../store/NotificationTypeList';
import * as DeleteNotificationType from '../store/DeleteNotificationType';

import "../../../fonts/style.css";
import './NotificationType.scss';

class NotificationType extends Component {
  constructor(props) {
    super(props);
    
    this.state = {
      isShowNewPopup: false,
      editingItem: null,
      isShowConfirm: false      
    };    
  }

  newPopupToggle = () => {
    this.setState(
      {
        isShowNewPopup: !this.state.isShowNewPopup
      },
      function(){
        if(!this.state.isShowNewPopup){
          this.props.getNotificationTypes();
        }
      }
    );
  }

  newEditPopup = (item) => {
    this.setState({
      editingItem: item,
      isShowNewPopup: true
    })
  } 

  confirmHandler = (isYes) => {
    if(isYes){
      var id = 1;
      this.props.deleteNotificationType(id).then(() => {
        this.confirmToggle();
        this.props.getNotificationTypes();
      });
    }
  }

  confirmToggle = () => {
    this.setState({
      isShowConfirm: !this.state.isShowConfirm
    })
  }  

  componentWillMount(){
    this.props.getNotificationTypes();
  }

  render() {
    return (
      <div>
        <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification Type"></NCBBreadcrum>
        <button className="btn-new" onClick={() => this.newEditPopup()}>Create New</button>  
        <table className="tbl">
          <thead>
            <tr>
              <th className="col-order">#</th>
              <th>Notification Type</th>
              <th>Recipient Type</th>
              <th>System</th>
              <th className="col-action">Action</th>
            </tr>
          </thead>
          <tbody>
            {
              this.props.notificationTypeList.notificationTypes.map((item, index) => 
                <tr key={index}>
                  <td className="text-center">{index + 1}</td>
                  <td>{item.notificationTypeName}</td>
                  <td>{item.recipientTypeName}</td>
                  <td>{item.systemName}</td>
                  <td className="text-center col-action">
                    <span className="icon-edit" onClick={() => this.newEditPopup(item)}></span>                    
                    <span className="icon-delete" onClick={this.confirmToggle}></span>
                  </td>
                </tr>
              )
            }            
          </tbody>
        </table>

        <NewNotificationType
          isShowPopup={this.state.isShowNewPopup}
          popupToggle={this.newPopupToggle}
          editingItem ={this.state.editingItem} 
        />

        <ConfirmPopup 
            confirmHandler={this.confirmHandler}
            confirmToggle={this.confirmToggle}
            isShowPopup={this.state.isShowConfirm}
            title={"Delete Notification Type"}
            content={"Are you sure you want to delete this notification type?"}
            warningIcon={false}
        ></ConfirmPopup>
      </div>
    );
  }
}

export default connect(
  state => {
    const {notificationTypeList, deleteNotificationType} = state;
    return {
      notificationTypeList,
      deleteNotificationType
    };
  },
  dispatch => bindActionCreators(Object.assign({}, NotificationTypeList.actionCreators, DeleteNotificationType.actionCreators), dispatch)
)(NotificationType);
