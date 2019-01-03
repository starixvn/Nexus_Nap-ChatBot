import React, { Component } from 'react';
import { connect } from 'react-redux';
import { UncontrolledTooltip } from 'reactstrap';
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
      isShowConfirm: false,
      activeRowIndex: -1,
      deletingNotificationTypeName: ''
    };    
  }

  newPopupToggle = () => {
    this.setState(
      {
        isShowNewPopup: !this.state.isShowNewPopup,
        activeRowIndex: this.state.isShowNewPopup ? -1 : this.state.activeRowIndex
      },
      function(){
        if(!this.state.isShowNewPopup){
          this.props.getNotificationTypes();
        }
      }
    );
  }

  newEditPopup = (item, rowIndex) => {
    this.setState({
      editingItem: item,
      activeRowIndex: rowIndex,
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

  confirmToggle = (rowIndex, deletingNotificationTypeName) => {
    this.setState({
      isShowConfirm: !this.state.isShowConfirm,
      activeRowIndex: rowIndex,
      deletingNotificationTypeName: deletingNotificationTypeName
    })
  }  

  componentWillMount(){
    this.props.getNotificationTypes();
  }

  render() {
    console.log(this.state.activeRowIndex);
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
                <tr key={index} className={index == this.state.activeRowIndex ? 'active' : ''}>
                  <td className="text-center">{index + 1}</td>
                  <td>{item.notificationTypeName}</td>
                  <td>{item.recipientTypeName}</td>
                  <td>{item.systemName}</td>
                  <td className="text-center col-action">
                    <span className="icon-edit" id={"edit-" + index} onClick={() => this.newEditPopup(item, index)}></span>                    
                    <UncontrolledTooltip placement="top" target={"edit-" + index}>Edit</UncontrolledTooltip>
                    <span className="icon-delete" id={"delete-" + index} onClick={() => this.confirmToggle(index, item.notificationTypeName)}></span>
                    <UncontrolledTooltip placement="top" target={"delete-" + index}>Delete</UncontrolledTooltip>
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
            content={'<p>Are you sure you want to delete <br /> <strong>"' + this.state.deletingNotificationTypeName + '"</strong></p>' }
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
