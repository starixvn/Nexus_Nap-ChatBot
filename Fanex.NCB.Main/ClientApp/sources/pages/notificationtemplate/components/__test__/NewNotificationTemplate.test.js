import React from 'react';
import { NewNotificationTemplate } from '../NewNotificationTemplate';
import { shallow } from 'enzyme';
import TemplatePreview from '../../../common/TemplatePreview/index';
import ResponseMessage from '../../../common/response-message/ResponseMessage';
import { FormFeedback } from 'reactstrap';
import ReactMde from "react-mde";

describe('NewNotificationTemplate', () => {

    afterAll(() => {
        jest.clearAllMock();
    });

    beforeAll(() => {
        jest.useFakeTimers();
    });

    describe('toggleHandler', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                toggleHandler: jest.fn()
            };

            // Act
            const container = shallow(<NewNotificationTemplate {...props}/>);
            container.instance().toggleHandler();

            // Assert
            expect(props.toggleHandler).toHaveBeenCalled();
        });
    });

    describe('renderNotificationTypeOption', () => {
        it('should return exactly template', () => {
            // Arrange
            const item = {
                notificationTypeID: 1,
                notificationTypeName: 'License'
            }

            // Act
            const container = shallow(<NewNotificationTemplate />);
            const actual = container.instance().renderNotificationTypeOption(item);

            // Assert
            expect(actual).toEqual(<option key={1} value={1}>License</option>);
        });
    });

    describe('handleGerneratePreview', () => {
        it('should return exactly template', () => {
            // Arrange
            const markdown = 'Internal';
            const promise = Promise.resolve(() => <TemplatePreview text={markdown} />);

            // Act
            const container = shallow(<NewNotificationTemplate />);
            const actual = container.instance().handleGerneratePreview(markdown);

            // Assert
            expect(actual).toEqual(promise);
        });
    });

    describe('renderForm', () => {
        const params = {
            values: {
                notificationTypeId: 1,
                notificationTemplateName: 'License',
                title: 'title',
                content: 'content',
                mode: '1'
            },
            errors: {
                notificationTypeId: true,
                notificationTemplateName: true,
                title: true,
                content: true
            },
            touched: {
                notificationTypeId: true,
                notificationTemplateName: true,
                title: true,
                content: true
            },
            handleChange: jest.fn(),
            setFieldValue: jest.fn(),
            isSubmitting: jest.fn()
        };

        it('should renderForm exactly FormFeedback', () => {
            //Arrange
            const props = {
                openMarkdownRulesPopup: jest.fn(),
                notificationTypes: [{ notificationTypeID: 1, notificationTypeName: 'test' }]
            }

            //Act
            const container = shallow(<NewNotificationTemplate {...props}/>).instance();
            const renderForm = shallow(container.renderForm({...params}));

            //Assert
            expect(renderForm.find(FormFeedback).length).toBe(4);
        });

        it('should renderForm exactly ReactMde onChange', () => {
            //Arrange
            const props = {
                openMarkdownRulesPopup: jest.fn(),
                notificationTypes: [{ notificationTypeID: 1, notificationTypeName: 'test' }]
            }

            //Act
            const container = shallow(<NewNotificationTemplate {...props}/>).instance();
            const renderForm = shallow(container.renderForm({...params}));
            renderForm.find(ReactMde).simulate('change', 'test');

            //Assert
            expect(params.setFieldValue).toHaveBeenCalledWith('content', 'test');
        });

        it('should renderForm exactly ReactMde onTabChange', () => {
            //Arrange
            const props = {
                openMarkdownRulesPopup: jest.fn(),
                notificationTypes: [{ notificationTypeID: 1, notificationTypeName: 'test' }]
            }

            //Act
            const container = shallow(<NewNotificationTemplate {...props}/>).instance();
            const renderForm = shallow(container.renderForm({...params}));
            renderForm.find(ReactMde).invoke('onTabChange')('test');

            //Assert
            expect(params.setFieldValue).toHaveBeenCalledWith('mode', 'test');
        });
    });

    describe('handleSubmit', () => {
        it('should call exactly props function when successful', async () => {
            // Arrange
            const props = {
                createTemplate: jest.fn(() => Promise.resolve({})),
                onCreateSuccess: jest.fn(),
                createTemplateResult: { isSuccessful: true }
            };
            const values = 1;
            const setSubmitting = jest.fn();

            // Act
            const container = shallow(<NewNotificationTemplate {...props}/>);
            await container.instance().handleSubmit(values, {setSubmitting});
            jest.advanceTimersByTime(1000);
            
            // Assert
            expect(setSubmitting).toHaveBeenCalledWith(true);
            expect(setTimeout).toHaveBeenCalledTimes(1);
            expect(props.onCreateSuccess).toHaveBeenCalled();
        });

        it('should call exactly props function when not successful', async () => {
            // Arrange
            const props = {
                createTemplate: jest.fn(() => Promise.resolve({})),
                onCreateSuccess: jest.fn(),
                createTemplateResult: { isSuccessful: false }
            };
            const values = 1;
            const setSubmitting = jest.fn();

            // Act
            const container = shallow(<NewNotificationTemplate {...props}/>);
            await container.instance().handleSubmit(values, {setSubmitting});
            
            // Assert
            expect(setSubmitting.mock.calls[0][0]).toBe(true);
            expect(setSubmitting.mock.calls[1][0]).toBe(false);
        });
    });

    describe('render', () => {
        it('should render exactly', () => {
            // Arrange
            const props = { createTemplateResult: true };

            // Act
            const container = shallow(<NewNotificationTemplate {...props}/>);
            const render = shallow(container.instance().render());

            // Assert
            expect(render.find(ResponseMessage).length).toBe(1);
        });
    });
});