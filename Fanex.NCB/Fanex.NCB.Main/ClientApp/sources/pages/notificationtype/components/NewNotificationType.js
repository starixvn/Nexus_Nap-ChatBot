import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Modal, ModalHeader, ModalBody } from 'reactstrap';
import { Form, FormGroup, Label, Input, FormFeedback } from 'reactstrap';

import { bindActionCreators } from 'redux';
import {ActionCreators} from '../actions/index';

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
    let showSucessMsg = this.props.addNotificationTypeData && this.props.addNotificationTypeData.addedresult;
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
            
            <Form className="form-create">
                <FormGroup>
                    <Label for="name" className="form-label required">Notification Type Name</Label>
                    <Input invalid value={currentNotificationTypeName} id="name" />
                    <FormFeedback>This field is required</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="recipientType" className="form-label required">Recipient Type</Label>
                    <Input type="select" name="select" id="recipientType" invalid>
                        <option>---Please select---</option>
                        {
                            
                            this.props.notificationTypeSettingsData.recipientTypes 
                            ? this.props.notificationTypeSettingsData.recipientTypes.map((item, index) => 
                                <option key={index} value={item.recipientTypeId} >{item.recipientTypeName}</option>
                            )
                            : null
                        }
                    </Input>
                    <FormFeedback>This field is required</FormFeedback>
                </FormGroup>

                <FormGroup>
                    <Label for="system" className="form-label required">System</Label>
                    <Input type="select" name="select" id="system" invalid>
                        <option>---Please select---</option>
                        {
                            
                            this.props.notificationTypeSettingsData.notificationSystems 
                            ? this.props.notificationTypeSettingsData.notificationSystems.map((item, index) => 
                                <option key={index} value={item.systemId}>{item.systemName}</option>
                            )
                            : null
                        }   
                    </Input>
                    <FormFeedback>This field is required</FormFeedback>
                </FormGroup>
                <button className="btn-submit" onClick={this.submitHandler}>Submit</button>
            </Form>
          </ModalBody>
        </Modal>        
    )
  }
}

function mapDispatchToProps(dispatch){
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state){
    const {notificationTypeSettingsData, addNotificationTypeData} = state;
    return Object.assign({}, {notificationTypeSettingsData, addNotificationTypeData});
}

export default connect(mapStateToProps, mapDispatchToProps)(NewNotificationType);