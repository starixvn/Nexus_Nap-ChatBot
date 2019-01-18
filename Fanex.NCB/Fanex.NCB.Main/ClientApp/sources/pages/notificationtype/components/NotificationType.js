import React, { Component } from 'react';
import { connect } from 'react-redux';
import { UncontrolledTooltip } from 'reactstrap';
import NCBBreadcrum from '../../common/breadcrumbs';
import ConfirmPopup from "../../common/ConfirmPopup";
import { bindActionCreators } from 'redux';
import {ActionCreators} from '../actions/index';
import NewNotificationType from './NewNotificationType';

import "../../../fonts/style.css";
import './NotificationType.scss';

export class NotificationType extends Component {
  constructor(props) {
    super(props);
    
    this.state = {
      isShowPopup: false,
      editingItem: null,
      isShowConfirm: false,
      activeRowIndex: -1,
      deletingNotificationTypeName: ''
    };    
  }

  popupToggle = () => {
    this.setState(
      {
        isShowPopup: !this.state.isShowPopup,
        activeRowIndex: this.state.isShowPopup ? -1 : this.state.activeRowIndex
      },
      function(){
        !this.state.isShowPopup && this.props.getNotificationTypes();        
      }
    );
  }

  openPopup = (item, rowIndex) => {
    this.setState({
      editingItem: item,
      activeRowIndex: rowIndex,
      isShowPopup: true
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
    console.log("toggle");
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
    return (
      <div>
        <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification Type 1"></NCBBreadcrum>
        <button className="btn-new" onClick={() => this.openPopup()}>Create New</button>  
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
                this.props.notificationTypeListData.length &&
                  this.props.notificationTypeListData.map((item, index) => 
                    <tr key={index} className={index == this.state.activeRowIndex ? 'active' : ''}>
                    <td className="text-center">{index + 1}</td>
                    <td>{item.notificationTypeName}</td>
                    <td>{item.recipientTypeName}</td>
                    <td>{item.systemName}</td>
                    <td className="text-center col-action">
                        <span className="icon-edit" id={"edit-" + index} onClick={() => this.openPopup(item, index)}></span>                    
                        <UncontrolledTooltip placement="top" target={"edit-" + index}>Edit</UncontrolledTooltip>
                        <span className="icon-delete" id={"delete-" + index} onClick={() => this.confirmToggle(index, item.notificationTypeName)}></span>
                        <UncontrolledTooltip placement="top" target={"delete-" + index}>Delete</UncontrolledTooltip>
                    </td>
                    </tr>                
                  )                
            }            
          </tbody>
        </table>
       
        {
          this.state.isShowPopup && 
            <NewNotificationType
              isShowPopup={this.state.isShowPopup}
              popupToggle={this.popupToggle}
              editingItem ={this.state.editingItem} />
        }

        {
          this.state.isShowConfirm &&
            <ConfirmPopup 
              confirmHandler={this.confirmHandler}
              confirmToggle={this.confirmToggle}
              isShowPopup={this.state.isShowConfirm}
              title={"Delete Notification Type"}
              content={'<p>Are you sure you want to delete <br /> <strong>"' + this.state.deletingNotificationTypeName + '"</strong></p>' }
              warningIcon={false} ></ConfirmPopup>            
        }
      </div>
    );
  }
}

function mapDispatchToProps(dispatch){
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state){
    const {notificationTypeListData, deleteNotificationTypeData} = state;
    return Object.assign({}, {notificationTypeListData, deleteNotificationTypeData});
}

export default connect(mapStateToProps, mapDispatchToProps)(NotificationType);
  