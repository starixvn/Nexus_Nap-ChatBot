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
            recipientTypeId: {
                value: this.props.updatePopupData.editingItem.recipientTypeId,
                isValid: true
            },
            isSummited: false
        }
    }  

    submitHandler = (e) => {
        e.preventDefault();

      
    }

    getParams = () => {
        return {
           
        };
    }

    checkValidParams = () => {
        return this.state.notificationTypeName.isValid && this.state.recipientTypeIds.isValid && this.state.systemId.isValid;
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

    recipientTypeSelected = () => {

    }

    render() {
        if (!this.props.updatePopupData.isShowed) {
            return null;
        }

        let popupTitle = this.props.updatePopupData.popupTitle;
        console.log(this.props.updatePopupData);

        let recipients = [];
        let recipientTypeId = this.state.recipientTypeId.value;
        recipientTypeId && this.props.settingData.recipients.map((item, index) => {
            if (item.recipientTypeId == recipientTypeId) {
                recipients.push(item);
            }
        })
        console.log(recipients);

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
                            <SelectField initialValue={this.props.updatePopupData.editingItem.recipientTypeId}
                                data={this.props.settingData.recipientTypes}
                                isSummited={this.state.isSummited}
                                onChange={(value) => this.onChange("recipientTypeId", value)} />  
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label required">Recipient</Label>
                            <MultipleSelectField
                                initialValue={this.props.updatePopupData.editingItem.recipientIds}
                                data={recipients}
                                keyField={"id"}
                                valueField={"name"}
                                statusField={"checked"}
                                isSummited={this.state.isSummited}
                                onChange={(value) => this.onChange("recipientIds", value)}
                            />
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
    const { settingData, updatePopupData, updateData } = state;
    return Object.assign({}, { settingData, updatePopupData, updateData });
}

export default connect(mapStateToProps, mapDispatchToProps)(UpdatePopup);