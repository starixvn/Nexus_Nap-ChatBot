import './EditNotification.scss';

import React from 'react';
import { DateTime } from 'luxon';
import { Form, FormGroup, Label, Button, Container, Row, Col, Input, FormFeedback } from 'reactstrap';
import { stringify } from 'qs';
import axios from 'axios';

import { Formik } from 'formik';
import * as Yup from 'yup';
import { NCBBreadcrum } from '../common/breadcrumbs';
import TreeSelect from '../common/TreeSelect/index';
import DatePicker from '../common/DatePicker/index';
import Labels from '../common/Labels';
import { DateTimeFormat } from '../constants/constants';
import ResponseMessage from '../common/response-message/ResponseMessage';
import Site from '../lib/site';
import ConfirmPopup from '../common/ConfirmPopup';
import errorResponseHandler from '../lib/errorResponseHandler';

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

export default class EditNotification extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            notificationId: parseInt(document.getElementById('notification-id').value),
            notification: null,
            recipientOptions: null
        };
    }

    componentDidMount() {
        const promises = [
            axios(Site.resolveClientUrl('EditNotification/Get/' + this.state.notificationId))
        ];

        Promise.all(promises)
            .then(([notificationResponse]) => {
                const notification = notificationResponse.data;
                notification.configSendTime = new Date(notification.configSendTime);

                this.setState({
                    notification
                }, this.getRecipients);
            }).catch(errorResponseHandler);
    }

    initializeObj = recipientOptions => {
        const recipientIds = [];
        this.findRecipientTreeValue(recipientIds, recipientOptions);
        const notification = this.state.notification;

        return {
            title: notification.title,
            content: notification.content,
            recipients: {
                value: recipientIds,
                extra: recipientOptions.slice()
            },
            sendMode: notification.sendImmediately ? '1' : '2',
            sendDate: notification.configSendTime
        };
    }

    findRecipientTreeValue = (array, tree) => {
        debugger;

        for (let i = 0; i < tree.length; i++) {
            const item = tree[i];
            if (item.children && item.children.length > 0) {
                this.findRecipientTreeValue(array, item.children);
            } else {
                if (this.state.notification.recipientIds.some(r => r === item.realValue)) {
                    array.push(item.value);
                }
            }
        }
    }

    getRecipients = () => {
        const notification = this.state.notification;
        axios.get(Site.resolveClientUrl('EditNotification/GetRecipients'), {
            params: { notificationTypeId: notification.notificationTypeId, productId: notification.productId }
        }).then(response => {
            const initialObj = this.initializeObj(response.data);

            this.setState({ recipientOptions: response.data, initialObj: initialObj });
        }).catch(errorResponseHandler);
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
            sendDate: DateTime.fromJSDate(values.sendDate).toFormat(DateTimeFormat),
            sendImmediately: values.sendMode === '1',
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
        title: Yup.string().required(Labels.FieldRequired),
        content: Yup.string().required(Labels.FieldRequired),
        recipients: Yup.array().required(Labels.FieldRequired),
        sendMode: Yup.string(),
        sendDate: Yup.mixed().when('sendMode', (sendMode, schema) => sendMode === '2' ? schema.required(Labels.FieldRequired) : schema)
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
            <Form onSubmit={handleSubmit} className="edit-notification-form">
                <FormGroup row>
                    <Label for="notificationTypeId" className="form-label required" sm={2}>Notification Type</Label>
                    <Col sm={10}>
                        <Input type="select"
                            invalid={touched.notificationTypeId && errors.notificationTypeId}
                            name="notificationTypeId" disabled
                            onChange={this.handleNotificationTypeChange}
                            value={values.notificationTypeId}
                        >
                            <option value="" disabled hidden>{Labels.PleaseSelect}</option>
                        </Input>
                        {touched.notificationTypeId && errors.notificationTypeId && <FormFeedback>{errors.notificationTypeId}</FormFeedback>}
                    </Col>

                </FormGroup>
                <FormGroup row>
                    <Label for="productId" className="form-label" sm={2}>Product</Label>

                    <Col sm={10}>
                        <Input type="select"
                            value={values.productId}
                            name="productId" disabled
                            onChange={this.handleProductChange}
                        >
                            <option value="">{Labels.None}</option>
                        </Input>
                    </Col>
                </FormGroup>
                <FormGroup row>
                    <Label for="productId" className="form-label required" sm={2}>Recipients</Label>

                    <Col sm={10}>
                        <TreeSelect value={values.recipients.value} onChange={this.handleRecipientChange} placeholder={Labels.PleaseSelect}
                            treeData={this.state.recipientOptions} className={touched.recipients && errors.recipients && 'is-invalid'}
                            labelInValue={false}
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
                                className={touched.sendDate && errors.sendDate && 'is-invalid'}
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
        if (!this.state.notification || !this.state.recipientOptions) {
            return null;
        }

        return (
            <>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Edit Notification" />
                <div className="content edit-notification">
                    {this.props.createNotificationResult &&
                        <ResponseMessage isSuccessful={this.props.createNotificationResult.isSuccessful}
                            message={this.props.createNotificationResult.message || Labels.CreateNotificationSuccessfully} />
                    }

                    <Formik ref="form"
                        initialValues={this.state.initialObj}
                        render={this.renderForm}
                        validationSchema={this.schema}
                        onSubmit={this.handleSubmit}
                    />

                </div>

            </>
        );
    }
}