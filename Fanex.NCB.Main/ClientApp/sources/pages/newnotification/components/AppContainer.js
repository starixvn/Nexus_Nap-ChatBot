import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label, Input, FormFeedback } from 'reactstrap';
import { Button } from 'reactstrap';
import { NCBBreadcrum } from '../../common/breadcrumbs';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import './AppContainer.scss';
import Labels from '../../common/Labels';
import { SelectField } from '../../common/SelectField';
import { TextField } from '../../common/text-field/TextField';

export class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isSummited: false,
            template: {
                title: {
                    originTemplate: "",
                    middleTemplate: "",
                    processedTemplate: ""
                    
                },
                content: {
                    originTemplate: "",
                    middleTemplate: "",
                    processedTemplate: ""                    
                },
                params: []
            }            
        };

        this.props.getSettings();
        this.props.getTemplateTypes();
        
    }

    onChange = (fieldName, fieldValue) => this.setState({ [fieldName]: fieldValue, isChanged: true });    

    detectParams = (text) => {
        let paramRegex = /{(.+)}/g;
        let params = [];        

        let copiedText = text.slice(0);
        let match = paramRegex.exec(copiedText);
        while (match) {
            let matchText = match[0];
            let regex = new RegExp(matchText, "g");

            params.push({ name: matchText, value: matchText });            
            copiedText = copiedText.replace(regex, "");
            match = paramRegex.exec(copiedText);
        }

        return params;
    }

    findMatches = (regex, str, matches = []) => {
        const res = regex.exec(str);
        res && matches.push(res);// && this.findMatches(regex, str, matches);

        return matches;
    }

    createMiddleTemplate = (text) => {
        return text.replace(/{/g, '<span class="red">{').replace(/}/g, '}</span>');
    }

    getTemplate = () => {
        this.props.getTemplate().then(() => {
            console.log("template");
            console.log(this.props);

            let titleTemplate = this.props.template.title;            
            let titleMiddleTemplate = this.createMiddleTemplate(titleTemplate);            

            let contentTemplate = this.props.template.content;
            let contentMiddleTemplate = this.createMiddleTemplate(contentTemplate);            

            let params = this.detectParams(titleTemplate + contentTemplate);

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
        });
    }

    render() {
        let interTemplate = "<span class='red'>{UM time}</span>";
        let parameters = {"UM time": "12 am"};
        let processedTemplate = "<span class='red'>11 am</span>";

        
        if (!this.props.settingData.notificationTypes) {
            return null;
        }

        console.log("render");
        console.log(this.props);
        console.log(this.state);

        
        return (
            <div>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="New Notification" />
                <div className="content form">
                    <Form>
                        <FormGroup>
                            <Label for="notificationType" className="form-label required">Notification Type</Label>
                            <SelectField 
                                data={this.props.settingData.notificationTypes}
                                isSummited={this.state.isSummited}
                                onChange={(notificationTypeId) => { this.onChange("notificationTypeId", notificationTypeId); }}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label">Product</Label>
                            <SelectField
                                data={this.props.settingData.products}
                                isSummited={this.state.isSummited}
                                onChange={(notificationTypeId) => { this.onChange("notificationTypeId", notificationTypeId); }}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label">Notification Template</Label>
                            <SelectField
                                data={this.props.notificationTemplates}
                                isSummited={this.state.isSummited}
                                onChange={(notificationTypeId) => { this.getTemplate(notificationTypeId); this.onChange("notificationTypeId", notificationTypeId); }}
                            />
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label required">Title</Label>
                            <TextField
                                initialValue={this.state.template.title.processedTemplate}
                                required={false}
                                disabled
                            />                                  
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label required">Content</Label>
                            <TextField
                                initialValue={this.state.template.content.processedTemplate}
                                required={false}
                                disabled
                            />       
                        </FormGroup>
                        <FormGroup>
                            <Label for="notificationType" className="form-label required">Send Date/Time</Label>

                        </FormGroup>

                        <Button className="btn-submit">Save</Button>

                    </Form>
                </div>
            </div>
        );
    }
}

function mapDispatchToProps(dispatch) {
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state) {
    const { settingData, notificationTemplates, template  } = state;
    return Object.assign({}, { settingData, notificationTemplates, template });
}

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);