import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Modal, ModalHeader, ModalBody } from 'reactstrap';
import { Form, FormGroup, Label, Input, FormFeedback } from 'reactstrap';
import { bindActionCreators } from 'redux';
import {ActionCreators} from '../actions/index';

class NewNotificationType extends Component {
  constructor(props) {
    super(props);   
    
    this.state = {
        params: {},
        validTypeName: true,
        validRecipientType: true,
        validSystem: true
    }
  }

  submitHandler = (e) => {     
      e.preventDefault();
      
      if(this.checkValidParams()){
        this.props.editingItem 
            ? this.props.editNewNotificationType(this.state.params).then(this.closePopup)
            : this.props.addNewNotificationType(this.state.params).then(this.closePopup);       
      }
  }

  checkValidParams = () => {
      return this.state.validTypeName && this.state.validRecipientType && this.state.validSystem;
  }

  onFieldChanged = (e, invalidStateName, invalidValue) => {
    var params = {...this.state.params};
    params[e.target.name] = e.target.value;
    this.setState({
        params: params,
        [invalidStateName]: e.target.value.trim() != invalidValue
    });
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
        <Modal isOpen={true} toggle={this.props.popupToggle} wrapClassName="new-notification-type">
          <ModalHeader toggle={this.props.popupToggle}>{popupTitle}</ModalHeader>
          <ModalBody>
            {
                showSucessMsg && (this.props.editingItem 
                                    ? <div className="msg-success ">Edit Notification Type Successfully</div> 
                                    : <div className="msg-success ">Create New Notification Type Successfully</div>)                                
            }
            
            <Form className="form-create">
                <FormGroup>
                    <Label for="name" className="form-label required">Notification Type Name</Label>
                    <Input invalid={!this.state.validTypeName} name="notificationTypeName" defaultValue={currentNotificationTypeName} id="name" 
                        onChange={(e) => this.onFieldChanged(e, "validTypeName", "")}/>
                    {
                        !this.state.validTypeName && <FormFeedback>This field is required</FormFeedback>
                    }                    
                </FormGroup>
                <FormGroup>
                    <Label for="recipientType" className="form-label required">Recipient Type</Label>
                    <Input type="select" name="recipientType" id="recipientType" defaultValue={currentRecipientTypeId} 
                        invalid={!this.state.validRecipientType} 
                        onChange={(e) => this.onFieldChanged(e, "validRecipientType", "-1")}>
                        <option value="-1">---Please select---</option>
                        {                            
                            this.props.notificationTypeSettingsData.recipientTypes &&
                                this.props.notificationTypeSettingsData.recipientTypes.map((item, index) => 
                                    <option key={index} value={item.recipientTypeId} >{item.recipientTypeName}</option>
                                )                            
                        }
                    </Input>
                    {
                        !this.state.validRecipientType && <FormFeedback>This field is required</FormFeedback>
                    }                    
                </FormGroup>

                <FormGroup>
                    <Label for="system" className="form-label required">System</Label>
                    <Input type="select" name="system" id="system" defaultValue={currentSystemId} invalid={!this.state.validSystem}
                        onChange={(e) => this.onFieldChanged(e, "validSystem", "-1")}>
                        <option value="-1">---Please select---</option>
                        {                            
                            this.props.notificationTypeSettingsData.notificationSystems &&
                                this.props.notificationTypeSettingsData.notificationSystems.map((item, index) => 
                                    <option key={index} value={item.systemId}>{item.systemName}</option>
                                )                            
                        }   
                    </Input>
                    {
                        !this.validSystem && <FormFeedback>This field is required</FormFeedback>
                    }                    
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