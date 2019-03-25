import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Modal, ModalHeader, ModalBody } from 'reactstrap';
import { Form, FormGroup, Label } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { InputField } from '../../common/InputField';
import { SelectField } from '../../common/SelectField';
import { MultipleSelectField } from '../../common/MultipleSelectField';
import { getMappingClass } from '../../common/CommonFunctions';
import Labels from '../../common/Labels';

export class UpdatePopup extends Component {
    constructor(props) {
        super(props);

        this.state = {
            groupID: this.props.updatePopupData.editingItem.groupID,      
            conversationID: this.props.updatePopupData.editingItem.conversationID,      
            recipientTypeId: this.props.updatePopupData.editingItem.recipientTypeId,
            recipientIds: this.props.updatePopupData.editingItem.recipientIds,
            remark: this.props.updatePopupData.editingItem.remark,            
            isSummited: false
        }
    }  

    submitHandler = (e) => {
        e.preventDefault();

        !this.state.isSummited && this.setState({ isSummited: true });                
        if (this.checkValidParams()) {
            let params = this.getParams();
            this.props.update(params).then(this.closePopup);
        }
    }

    getParams = () => {
        return {
            groupID: this.state.groupID,
            conversationId: this.state.conversationID,
            recipientTypeId: this.state.recipientTypeId.value,
            recipientIds: this.state.recipientIds.value,
            remark: this.state.remark.value
        };
    }

    checkValidParams = () => {
        return this.state.recipientTypeId.isValid && this.state.recipientIds.isValid && this.state.remark.isValid;
    }

    onChange = (fieldName, fieldValue) => this.setState({ [fieldName]: fieldValue });

    closePopup = () => setTimeout(function () { this.props.closeUpdatePopupHandler(); }.bind(this), 1000);

    filterRecipients = (recipientTypeId) => {
        this.props.filterRecipients(this.props.settingData.recipients, recipientTypeId);
        this.setState({
            recipientIds: {
                value: '',
                isValid: false
            }
        });
    }

    render() {        
        if (!this.props.updatePopupData.isShowed || !this.props.filteredRecipients) {
            return null;
        }

        let isSucessMsgShown = this.props.updateData && this.props.updateData.addedresult;    
        let editingItem = this.props.updatePopupData.editingItem;

        let licenseeStatuses = [];
        let recipientIds = this.state.recipientIds.value.split(",");
        let filteredRecipients = this.props.filteredRecipients;
        filteredRecipients && filteredRecipients.map((v, i) => {
            let isChecked = recipientIds.includes(v.id.toString());
            v['checked'] = isChecked ? true : false;
            isChecked && v.status != '' && licenseeStatuses.push(v.status);
        });    

        return (
            <Modal isOpen={true} toggle={this.props.closeUpdatePopupHandler} wrapClassName="new-notification-type">
                <ModalHeader toggle={this.props.closeUpdatePopupHandler}>{Labels.EditMappingStatus}</ModalHeader>
                <ModalBody>
                    {isSucessMsgShown && <div className="msg-success ">{Labels.UpdateMappingStatusSuccessfully}</div>}            
                    <Form className="form-edit">
                        <FormGroup>
                            <Label className="form-label ">{Labels.BotName}</Label>
                            <Label className="form-label">{editingItem.botName}</Label>                            
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label ">{Labels.GroupName}</Label>
                            <Label className="form-label">{editingItem.groupName}</Label>
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label required">{Labels.RecipientType}</Label>
                            <SelectField initialValue={editingItem.recipientTypeId.value}
                                data={this.props.settingData.recipientTypes}
                                isSummited={this.state.isSummited}
                                onChange={(recipientTypeId) => { this.filterRecipients(recipientTypeId.value); this.onChange("recipientTypeId", recipientTypeId); }} />  
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label required">{Labels.Recipient}</Label>
                            <MultipleSelectField
                                id={"recipient-id"}
                                selectedValue={this.state.recipientIds.value}
                                data={filteredRecipients}
                                keyField={"id"}
                                valueField={"name"}
                                statusField={"checked"}
                                isSummited={this.state.isSummited}
                                onChange={(value) => this.onChange("recipientIds", value)}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label ">{Labels.LicenseeStatus}</Label>
                            <Label className="form-label">{licenseeStatuses.join(",")}</Label>
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label ">{Labels.MappingStatus}</Label>
                            <Label className="form-label"><span className={getMappingClass(editingItem.mappingStatusId)}>{editingItem.mappingStatusName}</span></Label>
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label required">{Labels.Remark}</Label>
                            <InputField initialValue={editingItem.remark.value}
                                isSummited={this.state.isSummited}
                                onChange={(value) => this.onChange("remark", value)} />                                     
                        </FormGroup>
                        <button className="btn-submit" onClick={this.submitHandler}>{Labels.Submit}</button>
                    </Form>
                </ModalBody>
            </Modal>
        )
    }
}


function mapDispatchToProps(dispatch) {
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state) {
    const { settingData, updatePopupData, updateData, filteredRecipients } = state;
    return Object.assign({}, { settingData, updatePopupData, updateData, filteredRecipients });
}

export default connect(mapStateToProps, mapDispatchToProps)(UpdatePopup);