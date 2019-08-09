import './AppContainer.scss';

import React from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label, Button, Container, Row, Col, Input, FormFeedback } from 'reactstrap';
import { stringify } from 'qs';
import { isEmpty } from 'lodash';
import { Formik } from 'formik';
import * as Yup from 'yup';
import { NCBBreadcrum } from '../../../common/breadcrumbs';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../../actions/index';
import TreeSelect from '../../../common/TreeSelect/index';
import DatePicker from '../../../common/DatePicker/index';
import { TemplateTextField } from '../../../common/template-text-field/TemplateTextField';
import Labels from '../../../common/Labels';
import ResponseMessage from '../../../common/response-message/ResponseMessage';
import Site from '../../../lib/site';
import ConfirmPopup from '../../../common/ConfirmPopup';

const SendModeOptions = [
    {
        value: '1',
        label: 'Send Immediately'
    },
    {
        value: '2',
        label: 'Send At'
    }
];

export class AppContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isSummited: false,
            manuallyEditted: false,
            template: {
                title: {
                    originTemplate: '',
                    middleTemplate: '',
                    processedTemplate: '',
                    finalTemplate: ''
                },
                content: {
                    originTemplate: '',
                    middleTemplate: '',
                    processedTemplate: '',
                    finalTemplate: ''
                },
                params: []
            },
        };
    }

    componentDidMount() {
        this.props.getProducts();
        this.props.getNotificationTypes();
    }

    componentWillReceiveProps(nextProps) {
        if (nextProps.template !== this.props.template) {
            this.changeTemplate(nextProps);
        }
    }

    onChange = (fieldName, fieldValue) => {
        this.setState({
            [fieldName]: fieldValue,
            isChanged: true
        });
    }

    detectParams = text => {
        const paramRegex = /{[^{}]+}/g;
        const params = [];

        let copiedText = text.slice(0);
        let match = copiedText.match(paramRegex);

        while (match) {
            const matchText = match[0];
            const regex = new RegExp(matchText, 'g');

            params.push({ name: matchText, value: { value: matchText, isValid: false } });
            copiedText = copiedText.replace(regex, '');
            match = copiedText.match(paramRegex);
        }

        return params;
    }

    createMiddleTemplate = text => text.replace(/{/g, '<span class="red">{').replace(/}/g, '}</span>');

    getTemplate = () => {
        this.props.getTemplate();
    }

    changeTemplate(props) {
        const titleTemplate = props.template.title;
        const titleMiddleTemplate = this.createMiddleTemplate(titleTemplate);
        const contentTemplate = props.template.content;
        const contentMiddleTemplate = this.createMiddleTemplate(contentTemplate);
        const params = this.detectParams(titleTemplate + contentTemplate);

        this.setState(
            {
                template: {
                    title: {
                        originTemplate: titleTemplate,
                        middleTemplate: titleMiddleTemplate,
                        processedTemplate: titleMiddleTemplate
                    },
                    content: {
                        originTemplate: contentTemplate,
                        middleTemplate: contentMiddleTemplate,
                        processedTemplate: contentMiddleTemplate
                    },
                    params: params
                }
            }
        );
    }

    splitArrayIntoTwoColumns = array => {
        const newArray = [];
        const length = array.length;
        for (let i = 0; i < length; i += 2) {
            newArray.push(array.slice(i, i + 2));
        }

        return newArray;
    }

    processTemplate = (text, params) => {
        let copiedText = text.slice(0);
        params.map(item => {
            const regex = new RegExp(item.name, 'g');
            copiedText = copiedText.replace(regex, item.value.value);
        });

        return copiedText;
    }

    updateParam = (name, value) => {
        const params = this.state.template.params.slice(0);
        for (let i = 0; i < params.length; i++) {
            if (params[i].name === name) {
                params[i].value = value;
                break;
            }
        }

        return params;
    }

    updateTemplate = (name, value) => {
        const params = this.updateParam(name, value);
        const titleProcessedTemplate = this.processTemplate(this.state.template.title.middleTemplate, params);
        const titleFinalTemplate = this.processTemplate(this.state.template.title.originTemplate, params);
        const contentProcessedTemplate = this.processTemplate(this.state.template.content.middleTemplate, params);
        const contentFinalTemplate = this.processTemplate(this.state.template.content.originTemplate, params);

        this.setState(
            prevState => ({
                ...prevState,
                template: {
                    title: {
                        ...prevState.template.title,
                        processedTemplate: titleProcessedTemplate,
                        finalTemplate: titleFinalTemplate
                    },
                    content: {
                        ...prevState.template.content,
                        processedTemplate: contentProcessedTemplate,
                        finalTemplate: contentFinalTemplate
                    },
                    params: params
                }
            })
        );
    }

    editManually = () => {
        this.setState({
            manuallyEditted: true
        });
    }

    onChangeParam = (name, value) => {
        this.updateTemplate(name, value);
    }

    editTitle = value => {
        console.log(value);
    }

    editContent = value => {
        console.log(value);
    }

    handleToggleConfirmationPopup = () => this.props.toggleConfirmationPopup(!this.props.confimrationPopup.isOpen)

    handleConfirm = () => {
        const formik = this.refs.form;
        const values = formik.state.values;

        const recipientIds = [];
        this.getRecipientIds(recipientIds, this.selectedNodes);

        const data = stringify({
            notificationTypeId: values.notificationTypeId,
            productId: values.productId,
            recipientIds: recipientIds.toString(),
            title: values.title,
            content: values.content,
            sendDate: values.sendMode === '1' ? new Date().toLocaleString('en', { timeZone: 'Asia/Shanghai' }) : values.sendDate,
            sendImmediately: values.sendMode === '1'
        });

        formik.setSubmitting(true);
        this.props.createNewNotification(data).then(response => {
            formik.setSubmitting(false);
            this.props.toggleConfirmationPopup(false);
            if (this.props.createNotificationResult.isSuccessful) {
                setTimeout(function () {
                    window.location = Site.resolveClientUrl('NotificationList');
                }, 3000);
            }
        });
    }

    getRecipientIds = (ids, collection) => {
        for (let i = 0; i < collection.length; i++) {
            const item = collection[i];
            if (item.children && item.children.length > 0) {
                this.getRecipientIds(ids, item.children);
            } else {
                ids.push(item.node.props.realValue);
            }
        }
    }

    schema = Yup.object().shape({
        notificationTypeId: Yup.string().required(Labels.FieldRequired),
        title: Yup.string().required(Labels.FieldRequired),
        content: Yup.string().required(Labels.FieldRequired),
        recipients: Yup.array().required(Labels.FieldRequired),
        sendMode: Yup.string(),
        sendDate: Yup.date().when('sendMode', (sendMode, schema) => sendMode === '2' ? schema.required(Labels.FieldRequired) : schema)
    })

    renderNotificationTypeOption = (item) => <option key={item.notificationTypeID} value={item.notificationTypeID} >{item.notificationTypeName}</option>

    renderProductOption = (item) => <option key={item.id} value={item.id} >{item.name}</option>

    renderSendModeOption = item => <option key={item.value} value={item.value} >{item.label}</option>;

    handleSubmit = (values) => {
        const formatDate = values.sendDate.toLocaleString('en-US',
            { hour12: true, month: '2-digit', day: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });

        const message = values.sendMode === '1' ? 'Do you want to send immediately the notification to the selected recipient?'
            : `Do you want to send the notification at ${formatDate} to the selected recipient?`;
        this.props.toggleConfirmationPopup(true, message);
    }

    handleNotificationTypeChange = (event) => {
        const formik = this.refs.form;
        this.props.getRecipients(event.target.value, formik.state.values.productId);
        formik.handleChange(event);
        if (formik.state.values.recipients.length > 0) {
            formik.setFieldValue('recipients', []);
        }
    }

    handleProductChange = (event) => {
        const formik = this.refs.form;
        this.props.getRecipients(formik.state.values.notificationTypeId, event.target.value);
        formik.handleChange(event);
        if (formik.state.values.recipients.length > 0) {
            formik.setFieldValue('recipients', []);
        }
    }

    handleRecipientChange = (value, label, extra) => {
        const formik = this.refs.form;
        this.selectedNodes = extra.allCheckedNodes;
        formik.setFieldValue('recipients', value);
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
            <Form onSubmit={handleSubmit} className="new-notification-form">
                <FormGroup row>
                    <Label for="notificationTypeId" className="form-label required" sm={2}>Notification Type</Label>
                    <Col sm={10}>
                        <Input type="select"
                            invalid={touched.notificationTypeId && errors.notificationTypeId}
                            name="notificationTypeId"
                            onChange={this.handleNotificationTypeChange}
                            value={values.notificationTypeId}
                        >
                            <option value="" />
                            {this.props.notificationTypes.map(this.renderNotificationTypeOption)}
                        </Input>
                        {touched.notificationTypeId && errors.notificationTypeId && <FormFeedback>{errors.notificationTypeId}</FormFeedback>}
                    </Col>

                </FormGroup>
                <FormGroup row>
                    <Label for="productId" className="form-label" sm={2}>Product</Label>

                    <Col sm={10}>
                        <Input type="select"
                            value={values.productId}
                            name="productId"
                            onChange={this.handleProductChange}
                        >
                            <option value="">{Labels.None}</option>
                            {this.props.products.map(this.renderProductOption)}
                        </Input>
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="productId" className="form-label required" sm={2}>Recipients</Label>

                    <Col sm={10}>
                        <TreeSelect value={values.recipients} onChange={this.handleRecipientChange}
                            invalid={touched.recipients && errors.recipients} treeData={this.props.recipientOptions}
                        />
                        {touched.recipients && errors.recipients && <FormFeedback>{errors.recipients}</FormFeedback>}
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="title" className="form-label required" sm={2}>Title</Label>
                    <Col sm={10}>
                        <Input value={values.title} name="title"
                            invalid={touched.title && errors.title}
                            onChange={handleChange}
                        />
                        {touched.title && errors.title && <FormFeedback>{errors.title}</FormFeedback>}
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="content" className="form-label required" sm={2}>Content</Label>

                    <Col sm={10}>
                        <Input className="notification-content" type="textarea" value={values.content} name="content"
                            onChange={handleChange} invalid={touched.content && errors.content}
                        />
                        {touched.content && errors.content && <FormFeedback>{errors.content}</FormFeedback>}
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="sendMode" className="form-label required" sm={2}>Send Date/Time</Label>
                    <Col sm={10}>
                        <div className="send-date-area">
                            <Input type='select'
                                value={values.sendMode}
                                name="sendMode"
                                onChange={handleChange}
                            >
                                {SendModeOptions.map(this.renderSendModeOption)}
                            </Input>
                            <DatePicker selected={values.sendDate} showTimeSelect disabled={values.sendMode === '1'}
                                onChange={value => setFieldValue('sendDate', value)}
                                invalid={touched.sendDate && errors.sendDate}
                            />
                            <span>(GMT +8)</span>
                            {touched.sendDate && errors.sendDate && <FormFeedback>{errors.sendDate}</FormFeedback>}
                        </div>
                    </Col>

                </FormGroup>
                <FormGroup row className="list-buttons">
                    <Button className="btn-submit offset-sm-2" disabled={isSubmitting}>Save</Button>
                </FormGroup>
            </Form >
        );
    }

    render() {
        if (!this.props.notificationTypes || !this.props.products) {
            return null;
        }

        return (
            <>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="New Notification" />

                <div className="content new-notification">
                    {!isEmpty(this.props.createNotificationResult) &&
                        <ResponseMessage isSuccessful={this.props.createNotificationResult.isSuccessful}
                            message={this.props.createNotificationResult.message || Labels.CreateNotificationSuccessfully} />
                    }

                    <Formik ref="form"

                        initialValues={{
                            notificationTypeId: '',
                            productId: '',
                            recipients: [],
                            title: '',
                            content: '',
                            sendDate: new Date(),
                            sendMode: '1'
                        }}
                        render={this.renderForm}
                        validationSchema={this.schema}
                        onSubmit={this.handleSubmit}
                    />

                </div>
                <ConfirmPopup
                    onConfirm={this.handleConfirm}
                    onToggle={this.handleToggleConfirmationPopup}
                    isOpen={this.props.confimrationPopup.isOpen}
                    content={this.props.confimrationPopup.content}
                />}
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    return {
        notificationTypes: state.notificationTypes,
        products: state.products,
        recipientOptions: state.recipients,
        createNotificationResult: state.createNotificationResult,
        confimrationPopup: state.confimrationPopup
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);