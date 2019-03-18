import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Modal, ModalHeader, ModalBody } from 'reactstrap';
import { Form, FormGroup, Label } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { InputField } from '../../common/InputField';
import { SelectField } from '../../common/SelectField';
import { MultipleSelectField } from '../../common/MultipleSelectField';

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

    onChange = (fieldName, fieldValue) => {
        this.setState({
            [fieldName]: fieldValue
        });
    }

    closePopup = () => {
        setTimeout(function () {
            this.props.closeUpdatePopupHandler();
        }.bind(this),
            1000);
    }

    filterRecipients = (recipientTypeId) => {
        this.props.filterRecipients(this.props.settingData.recipients, recipientTypeId);
    }

    render() {
        if (!this.props.updatePopupData.isShowed || !this.props.filteredRecipients) {
            return null;
        }

        let popupTitle = this.props.updatePopupData.popupTitle;

        console.log("render popoup");
        console.log(this.props.filteredRecipients);
        console.log(this.props.updatePopupData.editingItem.recipientIds.value);

        let filteredRecipients = [];
        let recipientTypeId = this.props.updatePopupData.editingItem.recipientTypeId.value;
        recipientTypeId && this.props.settingData.recipients.map((item, index) => {
            if (item.recipientTypeId == recipientTypeId) {
                filteredRecipients.push(item);
            }
        });  

        if (this.props.filteredRecipients) {
            this.props.filteredRecipients.map((v, i) => {
                v['checked'] = false;
                if (this.props.updatePopupData.editingItem.recipientIds.value.split(",").includes(v.id.toString())) {
                    v['checked'] = true;
                }
            });
        }
        
        return (
            <Modal isOpen={true} toggle={this.props.closeUpdatePopupHandler} wrapClassName="new-notification-type">
                <ModalHeader toggle={this.props.closeUpdatePopupHandler}>{popupTitle}</ModalHeader>
                <ModalBody>
                    <Form className="form-edit">
                        <FormGroup>
                            <Label className="form-label ">Bot Name</Label>
                            <Label className="form-label">{this.props.updatePopupData.editingItem.botName}</Label>                            
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label ">Group Name</Label>
                            <Label className="form-label">{this.props.updatePopupData.editingItem.groupName}</Label>
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label required">Recipient Type</Label>
                            <SelectField initialValue={this.props.updatePopupData.editingItem.recipientTypeId.value}
                                data={this.props.settingData.recipientTypes}
                                isSummited={this.state.isSummited}
                                onChange={(recipientTypeId) => { this.filterRecipients(recipientTypeId.value); this.onChange("recipientTypeId", recipientTypeId); }} />  
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label required">Recipient</Label>
                            <MultipleSelectField
                                id={"recipient-id"}
                                initialValue={this.props.updatePopupData.editingItem.recipientIds.value}
                                data={this.props.filteredRecipients}
                                keyField={"id"}
                                valueField={"name"}
                                statusField={"checked"}
                                isSummited={this.state.isSummited}
                                onChange={(value) => this.onChange("recipientIds", value)}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label ">Licensee Status</Label>
                            <Label className="form-label">{"N/A"}</Label>
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label ">Mapping Status</Label>
                            <Label className="form-label">{this.props.updatePopupData.editingItem.mappingStatusName}</Label>
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label required">Remark</Label>
                            <InputField initialValue={this.props.updatePopupData.editingItem.remark.value}
                                isSummited={this.state.isSummited}
                                onChange={(value) => this.onChange("remark", value)} />                                     
                        </FormGroup>
                        <button className="btn-submit" onClick={this.submitHandler}>Submit</button>
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