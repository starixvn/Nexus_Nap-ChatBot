import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { Button, Modal, ModalHeader, ModalBody, Form, FormGroup, Label, Input, FormFeedback, Col } from 'reactstrap';
import { Formik } from 'formik';
import { object, string } from 'yup';
import ReactMde from "react-mde";
import Labels from '../../common/Labels';
import { editorCommandList, invalidFieldClass } from '../../constants/constants';
import ResponseMessage from '../../common/response-message/ResponseMessage';
import TemplatePreview from '../../common/TemplatePreview/index';

class EditNotificationTemplate extends Component {
    constructor(props) {
        super(props);

        this.state = {
            template: null
        };
    }

    toggleHandler = () => {
        this.props.toggleHandler();
    }

    renderNotificationTypeOption = item => <option key={item.notificationTypeID} value={item.notificationTypeID} >{item.notificationTypeName}</option>

    schema = object().shape({
        notificationTypeId: string().required(Labels.FieldRequired),
        notificationTemplateName: string().required(Labels.FieldRequired).max(100),
        title: string().required(Labels.FieldRequired).max(300),
        content: string().required(Labels.FieldRequired)
    })

    handleGerneratePreview = markdown => {
        return Promise.resolve(
            <TemplatePreview text={markdown} />
        );
    }

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
                            onChange={handleChange}
                            value={values.notificationTypeId}
                        >
                            <option value="" disabled hidden>{Labels.PleaseSelect}</option>
                            {this.props.notificationTypes.map(this.renderNotificationTypeOption)}
                        </Input>
                        {touched.notificationTypeId && errors.notificationTypeId && <FormFeedback>{errors.notificationTypeId}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="title" className="form-label required">Template Name</Label>
                    <div>
                        <Input value={values.notificationTemplateName} name="notificationTemplateName"
                            invalid={touched.notificationTemplateName && errors.notificationTemplateName}
                            onChange={handleChange}
                        />
                        {touched.notificationTemplateName && errors.notificationTemplateName && <FormFeedback>{errors.notificationTemplateName}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="title" className="form-label required">Title</Label>
                    <div>
                        <Input value={values.title} name="title"
                            invalid={touched.title && errors.title}
                            onChange={handleChange}
                        />
                        {touched.title && errors.title && <FormFeedback>{errors.title}</FormFeedback>}
                    </div>
                </FormGroup>
                <FormGroup>
                    <Label for="content" className="form-label required">Content</Label>

                    <div>
                        <ReactMde className={touched.content && errors.content && invalidFieldClass}
                            value={values.content} commands={editorCommandList}
                            onChange={content => setFieldValue('content', content)}
                            selectedTab={values.mode}
                            onTabChange={selectedTab => setFieldValue('mode', selectedTab)}
                            generateMarkdownPreview={this.handleGerneratePreview}
                            maxEditorHeight={400}
                        />
                        {touched.content && errors.content && <FormFeedback>{errors.content}</FormFeedback>}
                        <div className="markdown-rules" onClick={this.props.openMarkdownRulesPopup}>
                            Markdown Rules: <span className="icon-book" />
                        </div>
                    </div>
                </FormGroup>
                <div className="list-button">
                    <Button className="btn-submit float-right" disabled={isSubmitting} type="submit">{Labels.Submit}</Button>
                </div>
            </Form >
        );
    }

    handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        this.props.editTemplate(values).then(() => {
            if (this.props.editTemplateResult.isSuccessful) {
                setTimeout(() => {
                    this.props.onEditSuccess();
                }, 1000);
            } else {
                setSubmitting(false);
            }
        });
    }

    handleClose = () => {
        this.props.setEditTemplateResult(null);
        this.props.setEditTemplatePopup({ show: false });
    }

    render() {
        return (
            <Modal isOpen toggle={this.handleClose} className="new-notification-template" backdrop="static">
                <ModalHeader toggle={this.handleClose}>Edit Notification Template</ModalHeader>
                <ModalBody>
                    {this.props.editTemplateResult &&
                        <ResponseMessage isSuccessful={this.props.editTemplateResult.isSuccessful}
                            message={this.props.editTemplateResult.message || Labels.SubmitSuccessfully}
                        />
                    }
                    <Formik ref="form"
                        initialValues={this.props.editTemplatePopup.template}
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
        editTemplate: ActionCreators.editTemplate,
        setEditTemplatePopup: ActionCreators.setEditTemplatePopup,
        openMarkdownRulesPopup: ActionCreators.openMarkdownRulesPopup,
        setEditTemplateResult: ActionCreators.setEditTemplateResult
    }, dispatch);
};

const mapStateToProps = state => {
    const { editTemplateResult, editTemplatePopup, notificationTypes } = state;
    return {
        editTemplateResult,
        editTemplatePopup,
        notificationTypes
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(EditNotificationTemplate);