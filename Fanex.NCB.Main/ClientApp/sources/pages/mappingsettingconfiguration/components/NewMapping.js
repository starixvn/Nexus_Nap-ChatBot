import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { Button, Modal, ModalHeader, ModalBody, Form, FormGroup, Label, Input, FormFeedback, Col } from 'reactstrap';
import { Formik } from 'formik';
import { object, string, boolean } from 'yup';
import Labels from '../../common/Labels';
import ResponseMessage from '../../common/response-message/ResponseMessage';

class NewMapping extends Component {
    constructor(props) {
        super(props);
    }

    toggleHandler = () => {
        this.props.toggleHandler();
    }

    schema = object().shape({
        notificationTypeId: string().required(Labels.FieldRequired),
        notificationTemplateId: string().required(Labels.FieldRequired),
        systemId: string().required(Labels.FieldRequired),
        recipientTypeId: string().required(Labels.FieldRequired),
        msgContentsType: string().required(Labels.FieldRequired),
        // isTest: boolean().required(Labels.FieldRequired)
    })

    handleNotificationTypeChange = event => {
        const formik = this.refs.form;
        this.props.getRecipients(event.target.value);
        this.props.getTemplates(event.target.value);
        this.props.getNotificationTypeRef(event.target.value);

        formik.setValues({
            ...formik.state.values,
            notificationTypeId: event.target.value,
            // recipients: [],
            // templateId: ''
        });
        // this.setState({ searchValue: '' });
    }

    handleTemplateChange = event => {
        const formik = this.refs.form;

        if (event.target.value) {
            const selectedTemplate = this.props.notificationTemplates.find(x => x.notificationTemplateId === parseInt(event.target.value, 10));

            const paramsInTitle = selectedTemplate.title.match(/{[_-\w\s]+}/g) || [];
            const paramsInContent = selectedTemplate.content.match(/{[_-\w\s]+}/g) || [];

            const paramNames = new Set([...paramsInTitle, ...paramsInContent]);

            formik.setValues({
                ...formik.state.values,
                templateId: event.target.value,
                params: [...paramNames].map(x => ({ name: x, value: '' })),
                title: selectedTemplate.title,
                content: selectedTemplate.content,
                canEditTitleContent: false
            });
        } else {
            formik.setValues({
                ...formik.state.values,
                templateId: event.target.value,
                params: [],
                title: '',
                content: '',
                canEditTitleContent: true
            });
        }
    }

    renderNotificationTypeOption = item => <option key={item.notificationTypeID} value={item.notificationTypeID} >{item.notificationTypeName}</option>

    renderNotificationTemplateOption = item => <option key={item.notificationTemplateId} value={item.notificationTemplateId} >{item.notificationTemplateName}</option>

    renderMsgContentsTypeOption = item => <option key={item.type} value={item.type} >{item.name}</option>

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting
    }) => {
        return (
            <Form onSubmit={handleSubmit}>
                <FormGroup>
                    <Label for="notificationTypeId" className="form-label required">Notification Type</Label>
                    <div>
                        <Input type="select"
                            invalid={touched.notificationTypeId && errors.notificationTypeId}
                            name="notificationTypeId"
                            onChange={this.handleNotificationTypeChange}
                            value={values.notificationTypeId}
                        >
                            <option value="" disabled hidden>{Labels.PleaseSelect}</option>
                            {this.props.notificationTypes && this.props.notificationTypes.map(this.renderNotificationTypeOption)}
                        </Input>
                        {touched.notificationTypeId && errors.notificationTypeId && <FormFeedback>{errors.notificationTypeId}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup className="row-has-reset">
                    <Label for="notificationTemplateId" className="form-label required">Notification Template</Label>
                    <div>
                        <Input type="select"
                            invalid={touched.notificationTemplateId && errors.notificationTemplateId}
                            name="notificationTemplateId"
                            onChange={this.handleTemplateChange}
                            value={values.notificationTemplateId}
                        >
                            <option value="" disabled hidden>{Labels.PleaseSelect}</option>
                            {this.props.notificationTemplates && this.props.notificationTemplates.map(this.renderNotificationTemplateOption)}
                        </Input>
                        {touched.notificationTemplateId && errors.notificationTemplateId && <FormFeedback>{errors.notificationTemplateId}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="systemId" className="form-label">System</Label>
                    <div>
                        <Input invalid={touched.systemId && errors.systemId}
                            name="systemId"
                            disabled={true}
                            value={this.props.notificationTypeRef ? this.props.notificationTypeRef.systemName : ''}
                        />
                        {touched.systemId && errors.systemId && <FormFeedback>{errors.systemId}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="recipientTypeId" className="form-label">Recipient Type</Label>
                    <div>
                        <Input invalid={touched.recipientTypeId && errors.recipientTypeId}
                            name="recipientTypeId"
                            disabled={true}
                            value={this.props.notificationTypeRef ? this.props.notificationTypeRef.recipientTypeName : ''}
                        />
                        {touched.recipientTypeId && errors.recipientTypeId && <FormFeedback>{errors.recipientTypeId}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="msgContentsType" className="form-label required">Msg Contents Type</Label>
                    <div>
                        <Input type="select"
                            invalid={touched.msgContentsType && errors.msgContentsType}
                            name="msgContentsType"
                            onChange={this.handleNotificationTypeChange}
                            value={values.msgContentsType}
                        >
                            <option value="" disabled hidden>{Labels.PleaseSelect}</option>
                            {this.props.msgContentsTypes && this.props.msgContentsTypes.map(this.renderMsgContentsTypeOption)}
                        </Input>
                        {touched.msgContentsType && errors.msgContentsType && <FormFeedback>{errors.msgContentsType}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="requiredParam" className="form-label required">Required Param</Label>
                    <div>
                        <Input invalid={touched.requiredParam && errors.requiredParam}
                            name="requiredParam"
                            onChange={this.handleChange}
                            value={values.requiredParam}
                        />
                        {touched.requiredParam && errors.requiredParam && <FormFeedback>{errors.requiredParam}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="isTest" className="form-label">Is Test</Label>
                    <div>
                        <Input invalid={touched.isTest && errors.isTest}
                            name="isTest"
                            onChange={this.handleChange}
                            value={values.isTest}
                        />
                        {touched.isTest && errors.isTest && <FormFeedback>{errors.isTest}</FormFeedback>}
                    </div>
                </FormGroup>
                <div className="list-button">
                    <Button className="btn-submit float-right" disabled={isSubmitting} type="submit">{Labels.Save}</Button>
                </div>
            </Form >
        );
    }

    handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        this.props.createSettingType(values).then(() => {
            if (this.props.createSettingTypeResult.isSuccessful) {
                setTimeout(() => {
                    this.props.onCreateSuccess();
                }, 1000);
            } else {
                setSubmitting(false);
            }
        });
    }

    componentDidMount = () => {
        this.props.getMsgContentsTypes();
    }

    render() {
        return (
            <Modal isOpen toggle={this.props.onClose} className="new-setting-type" backdrop="static">
                <ModalHeader toggle={this.props.onClose}>Create New Mapping Setting</ModalHeader>
                <ModalBody>
                    {this.props.createSettingTypeResult &&
                        <ResponseMessage isSuccessful={this.props.createSettingTypeResult.isSuccessful}
                            message={this.props.createSettingTypeResult.message || Labels.CreateNewMsgContentsTypeSuccessfully}
                        />
                    }
                    <Formik ref="form"
                        initialValues={{
                            notificationTypeId: ''
                        }}
                        render={this.renderForm}
                        validationSchema={this.schema}
                        onSubmit={this.handleSubmit}
                    />
                </ModalBody>
            </Modal>
        );
    }
}

const mapDispatchToProps = dispatch => {
    return bindActionCreators({
        createSettingType: ActionCreators.createSettingType,
        getRecipients: ActionCreators.getRecipients,
        getTemplates: ActionCreators.getTemplates,
        getMsgContentsTypes: ActionCreators.getMsgContentsTypes,
        getNotificationTypeRef: ActionCreators.getNotificationTypeRef
    }, dispatch);
};

const mapStateToProps = state => {
    const {
        createSettingTypeResult,
        notificationTypes,
        notificationTemplates,
        recipients,
        msgContentsTypes,
        notificationTypeRef
    } = state;

    return {
        createSettingTypeResult,
        notificationTypes,
        notificationTemplates,
        recipients,
        msgContentsTypes,
        notificationTypeRef
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(NewMapping);