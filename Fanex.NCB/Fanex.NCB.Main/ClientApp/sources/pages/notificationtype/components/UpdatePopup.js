import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Modal, ModalHeader, ModalBody } from 'reactstrap';
import { Form, FormGroup, Label, Input, FormFeedback } from 'reactstrap';
import { bindActionCreators } from 'redux';
import {ActionCreators} from '../actions/index';
import PropTypes from 'prop-types';

class UpdatePopup extends Component {
  constructor(props) {
    super(props);   
    
    this.state = {        
        params: {
            notificationTypeName: props.updatePopupData.editingItem.notificationTypeName,
            recipientTypeId: props.updatePopupData.editingItem.recipientTypeId,
            systemId: props.updatePopupData.editingItem.systemId
        },        
        isSummited: false
    }    
  }

  submitHandler = (e) => {     
      e.preventDefault();

      this.setState({isSummited: true});      
      if(this.checkValidParams()){
        this.props.update(this.state.params, this.props.updatePopupData.isNew).then(this.closePopup);        
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
        this.props.closeUpdatePopupHandler();
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
    if(!this.props.updatePopupData.isShowed){
        return null;
    }

    let popupTitle = this.props.updatePopupData.popupTitle;
    let successMsg = this.props.updatePopupData.successMsg;
    let showSucessMsg = this.props.updateData && this.props.updateData.addedresult;    

    return (
        <Modal isOpen={true} toggle={this.props.closeUpdatePopupHandler} wrapClassName="new-notification-type">
          <ModalHeader toggle={this.props.closeUpdatePopupHandler}>{popupTitle}</ModalHeader>
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
                            this.props.settingData.recipientTypes &&
                                this.props.settingData.recipientTypes.map((item, index) => 
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
                            this.props.settingData.notificationSystems &&
                                this.props.settingData.notificationSystems.map((item, index) => 
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

UpdatePopup.propTypes = {
    closeUpdatePopupHandler: PropTypes.func.isRequired
};

function mapDispatchToProps(dispatch){
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state){
    const {settingData, updatePopupData, updateData} = state;
    return Object.assign({}, {settingData, updatePopupData, updateData});
}

export default connect(mapStateToProps, mapDispatchToProps)(UpdatePopup);