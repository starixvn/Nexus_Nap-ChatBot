import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Modal, ModalHeader, ModalBody, Form, FormGroup, Label } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { SelectField } from '../../common/select-field/SelectField';
import { MultipleSelectField } from '../../common/multiple-select-field/MultipleSelectField';
import CommonFunctions from '../../common/common-functions/CommonFunctions';
import Labels from '../../common/Labels';
import ResponseMessage from '../../common/response-message/ResponseMessage';
import { TextField } from '../../common/text-field/TextField';
import ConfirmPopup from '../../common/confirm-popup/ConfirmPopup';
import { NotAvailabeId } from '../../constants/constants';

const CONFIRM_CLOSE = 1;
const CONFIRM_UPDATE = 2;

export class EditPopup extends Component {
    static defaultProps = {
        editPopupData: {
            isShowed: false,
            editingItem: {
                groupID: 1,
                conversationID: 1,
                recipientTypeId: 1,
                chatGroupTypeId: 1,
                recipientIds: '',
                remark: ''
            }
        },
        filteredRecipients: []
    };

    constructor(props) {
        super(props);

        this.state = {
            groupID: this.props.editPopupData.editingItem.groupID,
            conversationID: this.props.editPopupData.editingItem.conversationID,
            recipientTypeId: this.props.editPopupData.editingItem.recipientTypeId,
            recipientIds: this.props.editPopupData.editingItem.recipientIds,
            chatGroupTypeId: this.props.editPopupData.editingItem.chatGroupTypeId,
            remark: this.props.editPopupData.editingItem.remark,
            isSummited: false,
            isChanged: false,
            isShowedConfirm: false,
            confirmTitle: '',
            confirmContent: '',
            confirmType: 0
        };
    }

    submitHandler = e => {
        e.preventDefault();

        this.setSubmittedState();
        if (this.checkValidParams()) {
            if (CommonFunctions.isMapped(this.props.editPopupData.editingItem.mappingStatusId)) {
                this.setState({
                    isShowedConfirm: true,
                    confirmTitle: Labels.ConfirmMessage,
                    confirmContent: Labels.ConfirmUpdate,
                    confirmType: CONFIRM_UPDATE
                });
            }
            else {
                this.submitData();
            }
        }
    }

    submitData = () => {
        const params = this.getParams();
        this.props.editMapping(params).then(() => {
            if (this.props.editResult.isSuccessful) {
                this.closePopupOnSuccess();
            }
        });
    }

    setSubmittedState = () => {
        !this.state.isSummited && this.setState({ isSummited: true });
    }

    getParams = () => {
        return {
            groupID: this.state.groupID,
            conversationId: this.state.conversationID,
            recipientTypeId: this.state.recipientTypeId.value,
            recipientIds: this.state.recipientIds.value,
            chatGroupTypeId: this.state.chatGroupTypeId.value,
            remark: this.state.remark.value
        };
    }

    checkValidParams = () => this.state.recipientTypeId.isValid && this.state.recipientIds.isValid && this.state.chatGroupTypeId.isValid;

    onChange = (fieldName, fieldValue) => this.setState({ [fieldName]: fieldValue, isChanged: true });

    closePopupOnSuccess = () => setTimeout(function () { this.props.onClosePopup(); }.bind(this), 1000);

    confirmThenClose = () => {
        if (this.state.isChanged) {
            this.setState({
                isShowedConfirm: true,
                confirmTitle: Labels.ConfirmMessage,
                confirmType: CONFIRM_CLOSE,
                confirmContent: Labels.ConfirmClose
            });
        }
        else {
            this.props.onClosePopup();
        }
    }

    confirmHandler = () => {
        this.state.confirmType === CONFIRM_CLOSE && this.props.onClosePopup();
        this.state.confirmType === CONFIRM_UPDATE && this.submitData();

        this.handleCloseConfirmPopup();
    }

    handleCloseConfirmPopup = () => this.setState({ isShowedConfirm: false });

    filterRecipients = recipientTypeId => {
        this.props.filterRecipients({ recipients: this.props.settings.recipients, recipientTypeId });
        this.setState({
            recipientIds: {
                value: '',
                isValid: false
            }
        });
    }

    render() {
        if (!this.props.editPopupData.isShowed || !this.props.filteredRecipients) {
            return null;
        }

        const editingItem = this.props.editPopupData.editingItem;
        const licenseeStatuses = [];
        const recipientIds = this.state.recipientIds.value.split(',');
        const filteredRecipients = this.props.filteredRecipients.map(item => {
            const isChecked = recipientIds.includes(item.id.toString());
            isChecked && item.status && licenseeStatuses.push(item.status);
            return { ...item, checked: isChecked ? true : false };
        });
        const recipientType = this.props.settings.recipientTypes.filter(v => v.id !== NotAvailabeId);
        return (
            <Modal isOpen wrapClassName="new-notification-type">
                <ModalHeader toggle={this.confirmThenClose} className="bold-text">{Labels.EditMappingStatus}</ModalHeader>
                <ModalBody>
                    {this.props.editResult && !CommonFunctions.isEmptyObject(this.props.editResult) &&
                        <ResponseMessage message={this.props.editResult.message || Labels.UpdateMappingStatusSuccessfully} isSuccessful={this.props.editResult.isSuccessful} />
                    }
                    <Form className="form-edit" tag='div'>
                        <FormGroup>
                            <Label className="form-label">{Labels.BotService}</Label>
                            <Label className="form-label">{editingItem.channelName}</Label>
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label">{Labels.BotName}</Label>
                            <Label className="form-label">{editingItem.botName}</Label>
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label">{Labels.GroupName}</Label>
                            <Label className="form-label">{editingItem.groupName}</Label>
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label required">{Labels.ChatGroupType}</Label>
                            <SelectField initialValue={editingItem.chatGroupTypeId.value ? editingItem.chatGroupTypeId.value : 0}
                                data={this.props.settings.chatGroupTypes}
                                isSummited={this.state.isSummited}
                                onChange={chatGroupTypeId => { this.onChange('chatGroupTypeId', chatGroupTypeId); }}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label required">{Labels.RecipientType}</Label>
                            <SelectField initialValue={editingItem.recipientTypeId.value}
                                data={recipientType}
                                isSummited={this.state.isSummited}
                                onChange={recipientTypeId => {
                                    this.filterRecipients(recipientTypeId.value);
                                    this.onChange('recipientTypeId', recipientTypeId);
                                }}
                            />

                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label required">{Labels.Recipient}</Label>
                            <MultipleSelectField
                                id={'recipient-id'}
                                selectedValue={this.state.recipientIds.value}
                                data={filteredRecipients}
                                keyField={'id'}
                                valueField={'name'}
                                statusField={'checked'}
                                isSummited={this.state.isSummited}
                                isSearchable
                                onChange={value => this.onChange('recipientIds', value)}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label ">{Labels.LicenseeStatus}</Label>
                            <Label className="form-label">{CommonFunctions.getLicenseeStatus(licenseeStatuses.join(', '))}</Label>
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label">{Labels.Remark}</Label>
                            <TextField initialValue={editingItem.remark.value ? editingItem.remark.value : ''}
                                isSummited={this.state.isSummited}
                                onChange={value => this.onChange('remark', value)}
                                maxLength='200'
                                required={false}
                                placeholder={Labels.RemarkPlaceHolder}
                            />
                        </FormGroup>
                        <button type="submit" disabled={!this.state.isChanged} className="btn-submit" onClick={this.submitHandler}>{Labels.Save}</button>
                    </Form>
                </ModalBody>
                {this.state.isShowedConfirm &&
                    <ConfirmPopup
                        onConfirm={this.confirmHandler}
                        onToggle={this.handleCloseConfirmPopup}
                        isOpen={this.state.isShowedConfirm}
                        title={this.state.confirmTitle}
                        content={this.state.confirmContent}
                        warningIcon={false}
                    />}
            </Modal>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { settings, editPopupData, editResult, filteredRecipients } = state;
    return {
        settings,
        editPopupData,
        editResult,
        filteredRecipients
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(EditPopup);