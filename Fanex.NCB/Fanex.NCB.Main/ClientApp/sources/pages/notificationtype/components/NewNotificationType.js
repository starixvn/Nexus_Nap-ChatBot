import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Modal, ModalHeader, ModalBody } from 'reactstrap';
import { AvForm, AvField } from 'availity-reactstrap-validation';
import { bindActionCreators } from 'redux';
import * as AddNotificationType from '../store/AddNotificationType';
import * as NotificationTypeSetting from '../store/NotificationTypeSetting';

class NewNotificationType extends Component {
  constructor(props) {
    super(props);       
  }

  submitHandler = () => {      
      var params = {};  
      this.props.editingItem 
        ? this.props.editNewNotificationType(params).then(this.closePopup)
        : this.props.addNewNotificationType(params).then(this.closePopup);       
  }

  closePopup = () => {
    setTimeout(function(){
        this.props.popupToggle();
        this.props.refreshAddNotificationType();
    }.bind(this),
    1000);
  }

  componentWillMount(){
    this.props.getSettings();
  }  
  
  render() {   
    let popupTitle = this.props.editingItem ? "Edit Notification Type" : "Create New Notification Type";
    let showSucessMsg = this.props.addNotificationType && this.props.addNotificationType.addedresult;
    let currentNotificationTypeName = this.props.editingItem ? this.props.editingItem.notificationTypeName : '';
    let currentRecipientTypeId = this.props.editingItem ? this.props.editingItem.recipientTypeId : -1;
    let currentSystemId = this.props.editingItem ? this.props.editingItem.systemId : -1;

    return (
        <Modal isOpen={this.props.isShowPopup} toggle={this.props.popupToggle} wrapClassName="new-notification-type">
          <ModalHeader toggle={this.props.popupToggle}>{popupTitle}</ModalHeader>
          <ModalBody>
            {
                showSucessMsg ? (this.props.editingItem 
                                    ? <div className="msg-success ">Edit Notification Type Successfully</div> : 
                                    <div className="msg-success ">Create New Notification Type Successfully</div>) 
                               : null
            }
            
            <AvForm className="form-create">
                <AvField 
                    name="name" 
                    label="Notification Type Name" 
                    labelClass="form-label required"
                    value={currentNotificationTypeName}
                    required />

                <AvField 
                    type="select"
                    name="recipientType"
                    label="Recipient Type" 
                    labelClass="form-label required"
                    inputClass="notification-type"
                    value={currentRecipientTypeId}
                    required>

                    <option>Please select</option>
                    {
                        
                        this.props.notificationTypeSettings.settings 
                        ? this.props.notificationTypeSettings.settings.recipientTypes.map((item, index) => 
                            <option key={index} value={item.recipientTypeId} >{item.recipientTypeName}</option>
                        )
                        : null
                    }
                    
                </AvField>

                <AvField 
                    type="select" 
                    name="aystem"
                    labelClass="form-label required"
                    label="System" 
                    value={currentSystemId}
                    required>
                    <option>Please select</option>
                    {
                        
                        this.props.notificationTypeSettings.settings 
                        ? this.props.notificationTypeSettings.settings.notificationSystems.map((item, index) => 
                            <option key={index} value={item.systemId}>{item.systemName}</option>
                        )
                        : null
                    }                    
                </AvField>
                <button className="btn-submit" onClick={this.submitHandler}>Submit</button>
            </AvForm>
          </ModalBody>
        </Modal>        
    )
  }
}

export default connect(
    state => {
        const {addNotificationType, notificationTypeSettings} = state;
        return {
            addNotificationType,
            notificationTypeSettings
        };
      },
    dispatch => bindActionCreators(Object.assign({}, AddNotificationType.actionCreators, NotificationTypeSetting.actionCreators), dispatch)
)(NewNotificationType);
