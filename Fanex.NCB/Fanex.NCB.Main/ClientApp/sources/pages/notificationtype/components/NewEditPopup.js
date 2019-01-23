import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Modal, ModalHeader, ModalBody } from 'reactstrap';
import { Form, FormGroup, Label, Input, FormFeedback } from 'reactstrap';
import { bindActionCreators } from 'redux';
import {ActionCreators} from '../actions/index';
import PropTypes from 'prop-types';

class NewEditPopup extends Component {
  constructor(props) {
    super(props);   
    
    this.state = {        
        params: {
            notificationTypeName: props.newEditPopupData.editingItem.notificationTypeName,
            recipientTypeId: props.newEditPopupData.editingItem.recipientTypeId,
            systemId: props.newEditPopupData.editingItem.systemId
        },        
        isSummited: false
    }    
  }

  submitHandler = (e) => {     
      e.preventDefault();

      this.setState({isSummited: true});      
      if(this.checkValidParams()){
        this.props.updateNotificationType(this.state.params, this.props.newEditPopupData.isNew).then(this.closePopup);        
      }
  }  

  onFieldChanged = (e, fieldName) => {
    var params = {...this.state.params};
    params[fieldName] = e.target.value;
    this.setState({
        params: params
    });
  }  

  componentWillMount(){
    this.props.getSettings();
  }  

  closePopup = () => {
    setTimeout(function(){
        this.props.closeNewEditPopupHandler();
    }.bind(this),
    1000);
  }

  checkValidParams = () => {
    return this.isValidNotificationTypeName() && this.isValidRecipientType() && this.isValidSystem();
  }

  isValidNotificationTypeName = () => {
    return this.state.params.notificationTypeName && this.state.params.notificationTypeName != '';
  }

  isValidRecipientType = () => {
    return this.state.params.recipientTypeId && this.state.params.recipientTypeId != -1;
  }

  isValidSystem = () => {
    return this.state.params.systemId && this.state.params.systemId != -1;
  }
  
  render() {   
    console.log("child rerender");

    if(!this.props.newEditPopupData.isShowed){
        return null;
    }

    let popupTitle = this.props.newEditPopupData.popupTitle;
    let successMsg = this.props.newEditPopupData.successMsg;
    let showSucessMsg = this.props.addNotificationTypeData && this.props.addNotificationTypeData.addedresult;    

    return (
        <Modal isOpen={true} toggle={this.props.closeNewEditPopupHandler} wrapClassName="new-notification-type">
          <ModalHeader toggle={this.props.closeNewEditPopupHandler}>{popupTitle}</ModalHeader>
          <ModalBody>
            {
                showSucessMsg && <div className="msg-success ">{successMsg}</div> 
            }
            
            <Form className="form-create">
                <FormGroup>
                    <Label className="form-label required">Notification Type Name</Label>
                    <Input value={this.state.params.notificationTypeName} 
                        onChange={(e) => this.onFieldChanged(e, "notificationTypeName")}
                        invalid={this.state.isSummited && !this.isValidNotificationTypeName()} />
                    { this.state.isSummited && !this.isValidNotificationTypeName() && <FormFeedback>This field is required</FormFeedback> }                    
                </FormGroup>
                <FormGroup>
                    <Label className="form-label required">Recipient Type</Label>
                    <Input type="select"
                        value={this.state.params.recipientTypeId}
                        onChange={(e) => this.onFieldChanged(e, "recipientTypeId")}
                        invalid={this.state.isSummited && !this.isValidRecipientType()} >
                        <option value="-1">---Please select---</option>
                        {                            
                            this.props.notificationTypeSettingsData.recipientTypes &&
                                this.props.notificationTypeSettingsData.recipientTypes.map((item, index) => 
                                    <option key={index} value={item.recipientTypeId} >{item.recipientTypeName}</option>
                                )                            
                        }
                    </Input>
                    { this.state.isSummited && !this.isValidRecipientType() &&  <FormFeedback>This field is required</FormFeedback> }                    
                </FormGroup>
                <FormGroup>
                    <Label className="form-label required">System</Label>
                    <Input type="select"
                        value={this.state.params.systemId}
                        onChange={(e) => this.onFieldChanged(e, "systemId")}
                        invalid={this.state.isSummited && !this.isValidSystem()}>
                        <option value="-1">---Please select---</option>
                        {                            
                            this.props.notificationTypeSettingsData.notificationSystems &&
                                this.props.notificationTypeSettingsData.notificationSystems.map((item, index) => 
                                    <option key={index} value={item.systemId}>{item.systemName}</option>
                                )                            
                        }   
                    </Input>
                    { this.state.isSummited && !this.isValidSystem() &&  <FormFeedback>This field is required</FormFeedback>}                    
                </FormGroup>
                <button className="btn-submit" onClick={this.submitHandler}>Submit</button>
            </Form>
          </ModalBody>
        </Modal>        
    )
  }
}

NewEditPopup.propTypes = {
    closeNewEditPopupHandler: PropTypes.func.isRequired
};

function mapDispatchToProps(dispatch){
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state){
    const {notificationTypeSettingsData, addNotificationTypeData, newEditPopupData} = state;
    return Object.assign({}, {notificationTypeSettingsData, addNotificationTypeData, newEditPopupData});
}

export default connect(mapStateToProps, mapDispatchToProps)(NewEditPopup);