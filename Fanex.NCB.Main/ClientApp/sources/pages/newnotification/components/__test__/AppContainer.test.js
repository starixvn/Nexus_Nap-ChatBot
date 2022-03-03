import React from 'react';
import { shallow } from 'enzyme';
import { AppContainer } from '../AppContainer';
import ContentPreview from '../../../common/ContentPreview';
import { Label, Input, FormFeedback } from 'reactstrap';
import ReactMde from 'react-mde';
import DatePicker from '../../../common/DatePicker/index';

describe('AppContainer', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    beforeEach(() => {
        const messageContent = document.createElement('div');
        const headerText = document.createElement('div');
        messageContent.setAttribute('id', 'MessageContentLength');
        headerText.setAttribute('id', 'header-text');
        document.body.appendChild(messageContent);
        document.body.appendChild(headerText);
    });

    afterEach(() => {
        const messageContent = document.getElementById('MessageContentLength');
        const headerText = document.getElementById('header-text');
        if (messageContent) {
            document.body.removeChild(messageContent);
        }
        if (headerText) {
            document.body.removeChild(headerText);
        }
    });

    const initProps = {
        getProducts: jest.fn(),
        getNotificationTypes: jest.fn()
    };

    describe('componentDidMount', () => {
        it('should call exactly function', () => {
            // Arrange
            const props = { ...initProps };

            const container = shallow(<AppContainer {...props} />).instance();
            container.handleInitResendNotification = jest.fn();
            container.resendNotifcation = {
                notificationId: 1,
                isResendAll: true
            };

            // Act
            container.componentDidMount();

            // Assert
            expect(props.getProducts).toHaveBeenCalled();
            expect(props.getNotificationTypes).toHaveBeenCalled();
            expect(container.handleInitResendNotification).toHaveBeenCalled();
        });
    });

    describe('handleInitResendNotification', () => {
        it('should set state exactly', async () => {
            // Arrange
            const response = {
                resendNotification : {
                    recipients: [{ recipientId: 1 }],
                    platforms: [{ platformId: 1, platformName: 'test' }],
                    channelId: 2,
                    channelName: 'test',
                    notificationTypeId: 123,
                    title: 'good',
                    content: 'testing'
                },
                recipients: '1,2,3',
                unSelectableRecipients: []
            }
            const promise = Promise.resolve(response);
            const props = {
                ...initProps,
                setRecipients: jest.fn(),
                getResendNotification: jest.fn(() => promise)
            };

            const container = shallow(<AppContainer {...props} />).instance();
            container.resendNotifcation = {
                notificationId: 1,
                isResendAll: true
            };
            container.displayUnselectableRecipients = jest.fn();
            
            // Act
            container.handleInitResendNotification();
            await promise;

            // Assert
            expect(props.getResendNotification).toHaveBeenCalledWith(1,true);
        });
    });

    describe('displayUnselectableRecipients', () => {
        it('should set exactly html', () => {
            // Arrange
            const props = { ...initProps };
            const recipients = [{ recipientId: 1, recipientName: 'test' }];
            const expectedHtml = `<div class="warning-title">
                            <span class="icon-warning"></span> Unselectable recipient(s): test.</div>`;

            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            container.displayUnselectableRecipients(recipients);

            // Assert
            expect(document.getElementById('header-text').innerHTML).toEqual(expectedHtml);
        });
    });

    describe('handleToggleConfirmationPopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                confirmationPopup: { isOpen: false },
                setConfirmationPopup: jest.fn()
            };

            const container = shallow(<AppContainer {...props}/>).instance();
            container.refs = {
                form: { setSubmitting: jest.fn() }
            };

            // Act
            container.handleToggleConfirmationPopup();

            // Assert
            expect(props.setConfirmationPopup).toHaveBeenCalledWith({ isOpen: true });
        });
    });

    describe('handleToggleResetConfirmationPopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setResetConfirmationPopup: jest.fn()
            };

            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            container.handleToggleResetConfirmationPopup();

            // Assert
            expect(props.setResetConfirmationPopup).toHaveBeenCalledWith({ isOpen: false });
        });
    });

    describe('handleToggleEditConfirmationPopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setEditConfirmationPopup: jest.fn()
            };

            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            container.handleToggleEditConfirmationPopup();

            // Assert
            expect(props.setEditConfirmationPopup).toHaveBeenCalledWith({ isOpen: false });
        });
    });

    describe('handleConfirm', () => {
        it('should call exactly props function', async () => {
            // Arrange
            const promise = Promise.resolve({ isSuccessful: true });
            const props = {
                ...initProps,
                setConfirmationPopup: jest.fn(),
                createNewNotification: jest.fn(() => promise),
                recipientOptions: [{ id: 1, name: 'test' }]
            };
            const paramsExpected = 'channelIds=1%2C2&recipientIds=&title=test123&content=good%20evening&sendDate=Invalid%20DateTime&sendImmediately=false&platforms=1';

            const container = shallow(<AppContainer {...props}/>).instance();
            container.refs = {
                form: { 
                    state: {
                        values: {
                            channelIds: '1,2',
                            recipients: [{ id: 1, name: 'test' }],
                            title: 'test123',
                            content: 'good evening',
                            platforms: [{ value: 1 }]
                        }
                    }
                }
            };

            // Act
            container.handleConfirm();
            await promise;

            // Assert
            expect(props.setConfirmationPopup).toHaveBeenCalledWith({ isOpen: false });
            expect(props.createNewNotification).toHaveBeenCalledWith(paramsExpected);
        });
    });

    describe('renderNotificationTypeOption', () => {
        it('should return exactly template', () => {
            // Arrange
            const props = { ...initProps };
            const item = {
                notificationTypeID: 1,
                notificationTypeName: 'test'
            };
            const expected = <option key={1} value={1} >test</option>;

            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            const actual = container.renderNotificationTypeOption(item);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('renderNotificationTemplateOption', () => {
        it('should return exactly template', () => {
            // Arrange
            const props = { ...initProps };
            const item = {
                notificationTemplateId: 1,
                notificationTemplateName: 'test'
            };
            const expected = <option key={1} value={1} >test</option>;

            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            const actual = container.renderNotificationTemplateOption(item);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('renderProductOption', () => {
        it('should return exactly template', () => {
            // Arrange
            const props = { ...initProps };
            const item = {
                id: 1,
                name: 'test'
            };
            const expected = <option key={1} value={1} >test</option>;

            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            const actual = container.renderProductOption(item);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('renderSendModeOption', () => {
        it('should return exactly template', () => {
            // Arrange
            const props = { ...initProps };
            const item = {
                value: 1,
                label: 'test'
            };
            const expected = <option key={1} value={1} >test</option>;

            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            const actual = container.renderSendModeOption(item);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('handleSubmit', () => {
        it('should call exactly props function when valid content length with sendMode is 1', () => {
            // Arrange
            const props = {
                ...initProps,
                setConfirmationPopup: jest.fn()
            };
            const values = {
                content: 'test',
                sendDate: '',
                sendMode: '1'
            };
            const expected = 'Do you want to send the notification to the selected recipient(s) immediately?';

            const container = shallow(<AppContainer {...props}/>).instance();
            container.messageContentLength = 10;

            // Act
            container.handleSubmit(values);

            // Assert
            expect(props.setConfirmationPopup).toHaveBeenCalledWith({ isOpen: true, content: expected });
        });

        it('should call exactly props function when valid content length with sendMode not equal 1', () => {
            // Arrange
            const props = {
                ...initProps,
                setConfirmationPopup: jest.fn()
            };
            const values = {
                content: 'test',
                sendDate: '3/2/2022',
                sendMode: '5'
            };
            const expected = 'Do you want to send the notification to the selected recipient(s)  at 3/2/2022?';

            const container = shallow(<AppContainer {...props}/>).instance();
            container.messageContentLength = 10;

            // Act
            container.handleSubmit(values);

            // Assert
            expect(props.setConfirmationPopup).toHaveBeenCalledWith({ isOpen: true, content: expected });
        });

        it('should return when inValid content length', () => {
            // Arrange
            const props = {
                ...initProps,
                setConfirmationPopup: jest.fn()
            };
            const values = {
                content: 'test',
                sendDate: '3/2/2022',
                sendMode: '5'
            };

            const container = shallow(<AppContainer {...props}/>).instance();
            container.messageContentLength = 1;
            container.refs = {
                form: { setSubmitting: jest.fn() }
            };

            // Act
            container.handleSubmit(values);

            // Assert
            expect(container.refs.form.setSubmitting).toHaveBeenCalledWith(false);
        });
    });

    describe('handleNotificationTypeChange', () => {
        it('should call exactly props function when platforms > 0', async () => {
            // Arrange
            const props = {
                ...initProps,
                getPlatforms: jest.fn(),
                platforms: [{ platformId: 1, platformName: 'test'}],
                getRecipients: jest.fn(),
                getNotificationTemplates: jest.fn()
            };
            const mockEvent = { target: { value: 'test' }};

            const container = shallow(<AppContainer {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: {
                            productId: 1,
                            channelIds: '1,2'
                        }
                    },
                    setFieldValue: jest.fn(),
                    setValues: jest.fn()
                }
            };

            // Act
            await container.handleNotificationTypeChange(mockEvent);

            // Assert
            expect(props.getPlatforms).toHaveBeenCalledWith('test');
            expect(container.state.platformOptions).toEqual([{title: 'test', value: 1}]);
            expect(props.getRecipients).toHaveBeenCalledWith('test', '1', 1, '1,2');
            expect(props.getNotificationTemplates).toHaveBeenCalledWith('test');
            expect(container.refs.form.setFieldValue).toHaveBeenCalledWith('platforms', [{title: 'test', value: 1}]);
            expect(container.refs.form.setValues).toHaveBeenCalled();
            expect(container.state.searchValue).toEqual('');
            expect(container.state.searchPlatformValue).toEqual('');
        });

        it('should call exactly props function when platforms <= 0', async () => {
            // Arrange
            const props = {
                ...initProps,
                getPlatforms: jest.fn(),
                platforms: [],
                getRecipients: jest.fn(),
                getNotificationTemplates: jest.fn()
            };
            const mockEvent = { target: { value: 'test' }};

            const container = shallow(<AppContainer {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: {
                            productId: 1,
                            channelIds: '1,2'
                        }
                    },
                    setFieldValue: jest.fn(),
                    setValues: jest.fn()
                }
            };

            // Act
            await container.handleNotificationTypeChange(mockEvent);

            // Assert
            expect(props.getPlatforms).toHaveBeenCalledWith('test');
            expect(container.state.platformOptions).toEqual([]);
        });
    });

    describe('handleTemplateChange', () => {
        it('should set value when have param', () => {
            // Arrange
            const props = {
                ...initProps,
                notificationTemplates: [{ notificationTemplateId: 1, title: 'test', content: 'testing' }]
            };
            const mockEvent = { target: { value: '1' }};
            const expected = { canEditTitleContent: false, content: 'testing', id: 1, params: [], templateId: '1', title: 'test' };

            const container = shallow(<AppContainer {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: { id: 1 }
                    },
                    setValues: jest.fn()
                }
            };

            // Act
            container.handleTemplateChange(mockEvent);

            // Assert
            expect(container.refs.form.setValues).toHaveBeenCalledWith(expected);
        });

        
        it('should set value when not have param', () => {
            // Arrange
            const props = {
                ...initProps,
                notificationTemplates: [{ notificationTemplateId: 1, title: 'test', content: 'testing' }]
            };
            const mockEvent = { target: { value: null }};
            const expected = { canEditTitleContent: true, content: '', id: 1, params: [], templateId: null, title: '' };

            const container = shallow(<AppContainer {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: { id: 1 }
                    },
                    setValues: jest.fn()
                }
            };

            // Act
            container.handleTemplateChange(mockEvent);

            // Assert
            expect(container.refs.form.setValues).toHaveBeenCalledWith(expected);
        });
    });

    describe('handleProductChange', () => {
        it('should call exactly props function', async () => {
            // Arrange
            const props = {
                ...initProps,
                getRecipients: jest.fn(),
                recipientOptions: []
            };
            const mockEvent = { target: { value: 5 }};

            const container = shallow(<AppContainer {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: {
                            notificationTypeId: 1,
                            platforms: [{ value: 1, title: 'test' }],
                            channelIds: '1,2',
                            recipients: [{ id: 1 }]
                        }
                    },
                    setValues: jest.fn(),
                    setFieldValue: jest.fn()
                }
            };

            // Act
            await container.handleProductChange(mockEvent);

            // Assert
            expect(props.getRecipients).toHaveBeenCalledWith(1, '1', 5, '1,2');
            expect(container.refs.form.setFieldValue).toHaveBeenCalledWith('productId', 5);
            expect(container.refs.form.setFieldValue).toHaveBeenCalledWith('recipients', []);
            expect(container.refs.form.setFieldValue).toHaveBeenCalledWith('platforms', []);
        });
    });

    describe('handlePlatformChange', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                getRecipients: jest.fn(),
                recipientOptions: []
            };
            const value = [{ value: 1, title: 'test' }];

            const container = shallow(<AppContainer {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: {
                            notificationTypeId: 1,
                            channelIds: '1,2',
                            recipients: [{ id: 1 }],
                            productId: 5
                        }
                    },
                    setValues: jest.fn(),
                    setFieldValue: jest.fn()
                }
            };

            // Act
            container.handlePlatformChange(value);

            // Assert
            expect(props.getRecipients).toHaveBeenCalledWith(1, '1', 5, '1,2');
            expect(container.refs.form.setFieldValue).toHaveBeenCalledWith('recipients', []);
        });
    });

    describe('handleRecipientChange', () => {
        it('should set values exactly', () => {
            // Arrange 
            const props = { ...initProps };

            const container = shallow(<AppContainer {...props}/>).instance();
            container.refs = {
                form: {
                    setFieldValue: jest.fn()
                }
            };

            // Act
            container.handleRecipientChange('test',1,2);

            // Assert
            expect(container.refs.form.setFieldValue).toHaveBeenCalledWith('recipients', 'test');
        });
    });

    describe('handleResetTitleContent', () => {
        it('should call exactly props function', () => {
            // Arrange 
            const props = {
                ...initProps,
                setResetConfirmationPopup: jest.fn()
            };

            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            container.handleResetTitleContent();

            // Assert
            expect(props.setResetConfirmationPopup).toHaveBeenCalledWith({ isOpen: true, content: 'Reset Title and Content will recover the original template and discard your changes.' });
        });
    });

    describe('handleConfirmResetTitleContent', () => {
        it('should set values exactly', () => {
            // Arrange 
            const props = {
                ...initProps,
                notificationTemplates: [
                    {
                        notificationTemplateId: 1,
                        title: 'test',
                        content: 'test123'
                    }
                ],
                setResetConfirmationPopup: jest.fn()
            };

            const container = shallow(<AppContainer {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: {
                            templateId: 1,
                            params: [{ name: 'test' }]
                        }
                    },
                    setValues: jest.fn()
                }
            };

            // Act
            container.handleConfirmResetTitleContent();

            // Assert
            expect(props.setResetConfirmationPopup).toHaveBeenCalledWith({ isOpen: false });
            expect(container.refs.form.setValues).toHaveBeenCalledWith({ canEditTitleContent: false, content: 'test123', params: [{name: 'test', 'value': ''}], templateId: 1, title: 'test' });
        });
    });

    describe('handleEditTitleContent', () => {
        it('should call exactly props function', () => {
            // Arrange 
            const props = {
                ...initProps,
                setEditConfirmationPopup: jest.fn()
            };

            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            container.handleEditTitleContent();

            // Assert
            expect(props.setEditConfirmationPopup).toHaveBeenCalledWith({ isOpen: true, content: 'You can edit the Title and Content but the system doesn\'t support manually change the text in {}' });
        });
    });

    describe('handleConfirmEditTitleContent', () => {
        it('should set values exactly', () => {
            // Arrange 
            const props = {
                ...initProps,
                setEditConfirmationPopup: jest.fn()
            };

            const container = shallow(<AppContainer {...props}/>).instance();
            container.refs = {
                form: { setFieldValue: jest.fn() }
            };

            // Act
            container.handleConfirmEditTitleContent();

            // Assert
            expect(props.setEditConfirmationPopup).toHaveBeenCalledWith({ isOpen: false });
            expect(container.refs.form.setFieldValue).toHaveBeenCalledWith('canEditTitleContent', true);
        });
    });

    describe('handleReplaceParamsWithValue', () => {
        it('should set values exactly', () => {
            // Arrange 
            const props = {
                ...initProps,
                notificationTemplates: [
                    {
                        notificationTemplateId: 1,
                        title: 'test',
                        content: 'test123'
                    }
                ],
                setEditConfirmationPopup: jest.fn()
            };

            const container = shallow(<AppContainer {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: {
                            params: [{ value: 'test' }],
                            templateId: 1
                        }
                    },
                    setValues: jest.fn()
                }
            };

            // Act
            container.handleReplaceParamsWithValue();

            // Assert
            expect(container.refs.form.setValues).toHaveBeenCalledWith({ content: 'test123', params: [{value: 'test'}], templateId: 1, title: 'test' });
        });
    });

    describe('handleGerneratePreview', () => {
        it('should set values exactly', () => {
            // Arrange 
            const props = { ...initProps };
            const markdown = 'test';
            const expected = Promise.resolve(<ContentPreview text={test} />);

            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            const actual = container.handleGerneratePreview(markdown);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('onChangePlatformSearch', () => {
        it('should set state exactly', () => {
            // Arrange 
            const props = {
                ...initProps,
                platforms: [{ platformId: 1, platformName: 'test' }]
            };
            const value = 'test';
            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            container.onChangePlatformSearch(value);

            // Assert
            expect(container.state.searchPlatformValue).toEqual('test');
        });
    });

    describe('handleSearchPlatform', () => {
        it('should set values exactly', () => {
            // Arrange 
            const props = {
                ...initProps,
                getRecipients: jest.fn()
            };
            const value = 'test';
            const container = shallow(<AppContainer {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: {
                            platforms: [{ title: 'test', value: 1 }],
                            notificationTypeId: 1,
                            productId: 1,
                            channelIds: '1,2'
                        }
                    },
                    setFieldValue: jest.fn()
                }
            };
            container.setState({platformOptions: [{ title: 'test', value: 1 }]});

            // Act
            container.handleSearchPlatform(value);

            // Assert
            expect(container.refs.form.setFieldValue).toHaveBeenCalledWith('platforms',[{title: 'test', value: 1}, {label: 'test', value: 1}]);
            expect(container.state.searchPlatformValue).toEqual('');
            expect(props.getRecipients).toHaveBeenCalledWith(1, '1,1', 1, '1,2');
        });
    });

    describe('onChangeSearch', () => {
        it('should set state exactly', () => {
            // Arrange 
            const props = {
                ...initProps,
                recipientOptions: [{ id: 1, name: 'test' }]
            };
            const value = 'test';
            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            container.onChangeSearch(value);

            // Assert
            expect(container.state.searchValue).toEqual('test');
        });
    });

    describe('handleSearchRecipents', () => {
        it('should set values exactly', () => {
            // Arrange 
            const props = {
                ...initProps,
                recipientOptions: [{ value: 1, title: 'test' }],
                getRecipients: jest.fn()
            };
            const value = 'test';
            const container = shallow(<AppContainer {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: {
                            recipients: [{ value: 1, title: 'test' }]
                        }
                    },
                    setFieldValue: jest.fn()
                }
            };

            // Act
            container.handleSearchRecipents(value);

            // Assert
            expect(container.refs.form.setFieldValue).toHaveBeenCalledWith('recipients', [{title: 'test', value: 1}, {label: 'test', value: 1}]);
            expect(container.state.searchValue).toEqual('');
        });

        it('should set values exactly when value is empty', () => {
            // Arrange 
            const props = {
                ...initProps,
                recipientOptions: [{ value: 1, title: 'test' }],
                getRecipients: jest.fn()
            };
            const value = '';
            const container = shallow(<AppContainer {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: {
                            recipients: [{ value: 1, title: 'test' }]
                        }
                    },
                    setFieldValue: jest.fn()
                }
            };

            // Act
            container.handleSearchRecipents(value);

            // Assert
            expect(container.refs.form.setFieldValue).toHaveBeenCalledWith('recipients', [{title: 'test', value: 1}]);
            expect(container.state.searchValue).toEqual('');
        });
    });

    describe('isValidSearchRecipients', () => {
        it('should return expected result when recipient title equal label', () => {
            // Arrange 
            const props = { ...initProps };
            const result = [];
            const label = 'test';
            const recipient = { title: 'test', value: 1 };

            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            const actual = container.isValidSearchRecipients(result, label, recipient);

            // Assert
            expect(result).toEqual(['test']);
            expect(container.searchRecipients).toEqual([{ label: 'test', value: 1 }]);
            expect(actual).toEqual(true);
        });

        it('should return expected result when recipient title not equal label', () => {
            // Arrange 
            const props = { ...initProps };
            const result = [];
            const label = 'test';
            const recipient = { title: 'test123', value: 1, children: [{ title: 'test', value: 11 }] };

            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            const actual = container.isValidSearchRecipients(result, label, recipient);

            // Assert
            expect(actual).toEqual(true);
        });
    });

    describe('handleOpenMarkdownRulesPopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setMarkdownRulesPopup: jest.fn()
            };

            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            container.handleOpenMarkdownRulesPopup();

            // Assert
            expect(props.setMarkdownRulesPopup).toHaveBeenCalledWith({ isOpen: true });
        });
    });

    describe('handleCloseMarkdownRulesPopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setMarkdownRulesPopup: jest.fn()
            };

            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            container.handleCloseMarkdownRulesPopup();

            // Assert
            expect(props.setMarkdownRulesPopup).toHaveBeenCalledWith({ isOpen: false });
        });
    });

    describe('renderPageTitle', () => {
        it('should render resend title', () => {
            // Arrange
            const props = { ...initProps };

            const container = shallow(<AppContainer {...props}/>).instance();
            container.resendNotifcation = {
                notificationId: 1,
                isResendAll: true
            };

            // Act
            const actual = container.renderPageTitle();

            // Assert
            expect(actual).toEqual('Resend a notification from notification #1');
        });

        it('should render new title', () => {
            // Arrange
            const props = { ...initProps };

            const container = shallow(<AppContainer {...props}/>).instance();

            // Act
            const actual = container.renderPageTitle();

            // Assert
            expect(actual).toEqual('New Notification');
        });
    });

    describe('renderForm', () => {
        const params = {
            values: {
                content: '123',
                params: [{ name: 'test123', value: 1 }],
                canEditTitleContent: false,
                sendMode: '2'
            },
            errors: {
                notificationTypeId: true
            },
            touched: {
                notificationTypeId: true,

            },
            handleChange: jest.fn(),
            setFieldValue: jest.fn(),
            isSubmitting: jest.fn()
        };

        const props = {
            ...initProps,
            notificationTypes: [{ id: 1, name: 'test' }],
            products: [{ id: 1, name: 'AG' }],
            confirmationPopup: { isOpen: true },
            resetConfirmationPopup: { isOpen: true },
            editConfirmationPopup: { isOpen: true },
            markdownRulesPopup: { isOpen: true },
            notificationTemplates: [{ id: 1, name: 'test' }]
        };

        it('renderForm label exactly', () => {
            //Arrange
            const container = shallow(<AppContainer {...props}/>).instance();
            container.resendNotifcation = {
                notificationId: 1,
                isResendAll: true
            };
            container.handleNotificationTypeChange = jest.fn();

            //Act
            const renderForm = shallow(container.renderForm({...params}));
            renderForm.find(Input).at(0).simulate('change');

            //Assert
            expect(renderForm.find(Label).length).toBe(10);
            expect(renderForm.find(FormFeedback).length).toBe(1);
            expect(container.handleNotificationTypeChange).toHaveBeenCalled();
        });

        it('renderForm ReactMde exactly', () => {
            //Arrange
            const container = shallow(<AppContainer {...props}/>).instance();

            //Act
            const renderForm = shallow(container.renderForm({...params}));
            renderForm.find(ReactMde).at(0).simulate('change','test')
            renderForm.find(ReactMde).at(0).props().onTabChange('preview');

            //Assert
            expect(params.setFieldValue).toHaveBeenCalledWith('mode', 'preview');
            expect(params.setFieldValue).toHaveBeenCalledWith('content', 'test');
        });

        it('renderForm DatePicker exactly', () => {
            //Arrange
            const container = shallow(<AppContainer {...props}/>).instance();

            //Act
            const renderForm = shallow(container.renderForm({...params}));
            renderForm.find(DatePicker).at(0).simulate('change','03/03/2022')

            //Assert
            expect(params.setFieldValue).toHaveBeenCalledWith('sendDate', '03/03/2022');
        });
    });

    describe('render', () => {
        it('should render null when notificationTypes or products is null', () => {
            // Arrange
            const props = {
                ...initProps,
                notificationTypes: null,
                products: null
            };

            // Act
            const container = shallow(<AppContainer {...props} />);

            // Assert
            expect(container.type()).toEqual(null);
        });
    });
});