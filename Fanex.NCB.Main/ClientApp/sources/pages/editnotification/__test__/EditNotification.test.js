import React from 'react';
import { shallow } from 'enzyme';
import EditNotification from '../EditNotification';
import ContentPreview from '../../common/ContentPreview';
import { Label, FormFeedback } from 'reactstrap';
import ReactMde from 'react-mde';
import DatePicker from '../../common/DatePicker/index';
import Site from '../../lib/site';
import DataService from '../../lib/dataService';

describe('EditNotification', () => {

    beforeAll(() => {
        Site.resolveClientUrl = jest.fn(url => url);
    });

    afterAll(() => {
        jest.clearAllMocks();
    });

    beforeEach(() => {
        const notificationId = document.createElement('div');
        notificationId.setAttribute('id', 'notification-id');
        document.body.appendChild(notificationId);

        const messageContent = document.createElement('div');
        messageContent.setAttribute('id', 'MessageContentLength');
        document.body.appendChild(messageContent);
    });

    afterEach(() => {
        const notificationId = document.getElementById('notification-id');
        if (notificationId) {
            document.body.removeChild(notificationId);
        }

        const messageContent = document.getElementById('MessageContentLength');
        if (messageContent) {
            document.body.removeChild(messageContent);
        }
    });

    describe('componentDidMount', () => {
        it('should setState and call exactly props function', () => {
            // Arrange
            const container = shallow(<EditNotification />).instance();
            container.setState({ notificationId: 1 });

            // Act
            container.componentDidMount();

            // Assert
            expect(container.state.notification).toEqual(null);
        });
    });

    describe('initializeObj', () => {
        it('should return exactly', () => {
            // Arrange
            const notification = {
                recipients: [{ recipientName: 'test', recipientTypeId: 1, recipientId: 1, licenseeTypeId: 1}],
                platforms: [{ platformName: 'test123', platformId: 1 }],
                channelId: '1,2',
                channelName: 'channel',
                title: 'good',
                content: 'content',
                sendImmediately: true,
                configSendTime: '3/3/2022',
                productId: 2
            };
            const expected = { channelId: '1,2', channelName: 'channel', content: 'content', platforms: [{ title: 'test123', value: 1 }],
                productId: 2,recipients: [{ label: 'test', value: '1-1-1'}],sendDate: '3/3/2022', sendMode: '1', title: 'good' };

            const container = shallow(<EditNotification />).instance();
            container.setState({ notification: notification });

            // Act
            const actual = container.initializeObj();

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('getRecipients', () => {
        it('should setState exactly', () => {
            // Arrange
            const notificationId = 1, notificationTypeId = 2, productId = 3, platforms = [];
            const container = shallow(<EditNotification />).instance();

            // Act
            container.getRecipients(notificationId, notificationTypeId, productId, platforms);

            // Assert
            expect(container.state.recipientOptions).toEqual(null);
        });
    });

    describe('handleCloseConfirmationPopup', () => {
        it('should setState exactly', () => {
            // Arrange
            const container = shallow(<EditNotification />).instance();
            container.refs = {
                form: { setSubmitting: jest.fn() }
            };

            // Act
            container.handleCloseConfirmationPopup();

            // Assert
            expect(container.refs.form.setSubmitting).toHaveBeenCalledWith(false);
            expect(container.state.confimrationPopup).toEqual({ isOpen: false });
        });
    });

    describe('handleConfirm', () => {
        it('should call exactly function', async () => {
            // Arrange
            const recipientOptions = [{ value: 1, title: 'test' }];
            const response = { data: { isSuccessful: true, message: 'success' } };
            const promise = Promise.resolve(response);
            DataService.postDataAsync = jest.fn(() => promise);

            const container = shallow(<EditNotification />).instance();
            container.handleCloseConfirmationPopup = jest.fn();
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
                    },
                    setSubmitting: jest.fn()
                }
            };
            container.setState({ recipientOptions: recipientOptions});

            // Act
            await container.handleConfirm();

            // Assert
            expect(container.refs.form.setSubmitting).toHaveBeenCalledWith(false);
            expect(container.handleCloseConfirmationPopup).toHaveBeenCalled();
            expect(container.state.editNotificationResult).toEqual({ isSuccessful: true, message: 'success' });
        });
    });

    describe('renderSendModeOption', () => {
        it('should return exactly template', () => {
            // Arrange
            const item = {
                value: 1,
                label: 'test'
            };
            const expected = <option key={1} value={1} >test</option>;

            const container = shallow(<EditNotification />).instance();

            // Act
            const actual = container.renderSendModeOption(item);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('handleSubmit', () => {
        it('should call exactly props function when valid content length with sendMode is 1', () => {
            // Arrange
            const values = {
                content: 'test',
                sendDate: '',
                sendMode: '1'
            };
            const expected = 'Do you want to send the notification to the selected recipient(s) immediately?';

            const container = shallow(<EditNotification />).instance();
            container.messageContentLength = 10;

            // Act
            container.handleSubmit(values);

            // Assert
            expect(container.state.confimrationPopup).toEqual({ isOpen: true, message: expected });
        });

        it('should call exactly props function when valid content length with sendMode not equal 1', () => {
            // Arrange
            const values = {
                content: 'test',
                sendDate: '3/2/2022',
                sendMode: '5'
            };
            const expected = 'Do you want to send the notification to the selected recipient(s)  at 3/2/2022?';

            const container = shallow(<EditNotification />).instance();
            container.messageContentLength = 10;

            // Act
            container.handleSubmit(values);

            // Assert
            expect(container.state.confimrationPopup).toEqual({ isOpen: true, message: expected });
        });

        it('should return when inValid content length', () => {
            // Arrange
            const values = {
                content: 'test',
                sendDate: '3/2/2022',
                sendMode: '5'
            };

            const container = shallow(<EditNotification />).instance();
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

    describe('handleRecipientChange', () => {
        it('should set values exactly', () => {
            // Arrange 
            const container = shallow(<EditNotification />).instance();
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

    describe('handleGerneratePreview', () => {
        it('should set values exactly', () => {
            // Arrange 
            const markdown = 'test';
            const expected = Promise.resolve(<ContentPreview text={test} />);

            const container = shallow(<EditNotification />).instance();

            // Act
            const actual = container.handleGerneratePreview(markdown);

            // Assert
            expect(actual).toEqual(expected);
        });
    });

    describe('touchedAndErrors', () => {
        it('should return exactly', () => {
            // Arrange
            const touched = { name: 'test' };
            const errors = { name: 'test123' };
            const fieldName = 'name';

            const container = shallow(<EditNotification />).instance();

            // Act
            const actual = container.touchedAndErrors(touched, errors, fieldName);

            // Assert
            expect(actual).toBe('test123');
        });
    });

    describe('handleOpenMarkdownRulesPopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const container = shallow(<EditNotification />).instance();

            // Act
            container.handleOpenMarkdownRulesPopup();

            // Assert
            expect(container.state.markdownRulesPopup).toEqual({ isOpen: true });
        });
    });

    describe('handleCloseMarkdownRulesPopup', () => {
        it('should call exactly props function', () => {
            // Arrange
            const container = shallow(<EditNotification />).instance();

            // Act
            container.handleCloseMarkdownRulesPopup();

            // Assert
            expect(container.state.markdownRulesPopup).toEqual({ isOpen: false });
        });
    });

    describe('onChangeSearch', () => {
        it('should set state exactly', () => {
            // Arrange 
            const value = 'test';
            const container = shallow(<EditNotification />).instance();

            // Act
            container.onChangeSearch(value);

            // Assert
            expect(container.state.searchValue).toEqual('test');
        });
    });

    describe('handleSearchRecipents', () => {
        it('should set values exactly', () => {
            // Arrange 
            const value = 'test';
            const container = shallow(<EditNotification />).instance();
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
            container.setState({ recipientOptions: [{ value: 1, title: 'test' }] });

            // Act
            container.handleSearchRecipents(value);

            // Assert
            expect(container.refs.form.setFieldValue).toHaveBeenCalledWith('recipients', [{title: 'test', value: 1}, {label: 'test', value: 1}]);
            expect(container.state.searchValue).toEqual('');
        });

        it('should set values exactly when value is empty', () => {
            // Arrange 
            const value = '';
            const container = shallow(<EditNotification />).instance();
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
            container.setState({ recipientOptions: [{ value: 1, title: 'test' }] });

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
            const result = [];
            const label = 'test';
            const recipient = { title: 'test', value: 1 };

            const container = shallow(<EditNotification />).instance();

            // Act
            const actual = container.isValidSearchRecipients(result, label, recipient);

            // Assert
            expect(result).toEqual(['test']);
            expect(container.searchRecipients).toEqual([{ label: 'test', value: 1 }]);
            expect(actual).toEqual(true);
        });

        it('should return expected result when recipient title not equal label', () => {
            // Arrange 
            const result = [];
            const label = 'test';
            const recipient = { title: 'test123', value: 1, children: [{ title: 'test', value: 11 }] };

            const container = shallow(<EditNotification />).instance();

            // Act
            const actual = container.isValidSearchRecipients(result, label, recipient);

            // Assert
            expect(actual).toEqual(true);
        });
    });

    describe('handlePlatformChange', () => {
        it('should call exactly props function', () => {
            // Arrange
            const value = [{ value: 1, title: 'test' }];

            const container = shallow(<EditNotification />).instance();
            container.refs = {
                form: {
                    state: {
                        values: {
                            channelIds: '1,2',
                            recipients: [{ id: 1 }],
                            productId: 5
                        }
                    },
                    setValues: jest.fn(),
                    setFieldValue: jest.fn()
                }
            };
            container.setState({ notificationId: 1, notification: { notificationTypeId: 1 } });
            container.getRecipients = jest.fn();

            // Act
            container.handlePlatformChange(value);

            // Assert
            expect(container.getRecipients).toHaveBeenCalledWith(1, 1, 5, '1');
            expect(container.refs.form.setFieldValue).toHaveBeenCalledWith('recipients', []);
        });
    });

    describe('onChangePlatformSearch', () => {
        it('should set state exactly', () => {
            // Arrange 
            const value = 'test';
            const container = shallow(<EditNotification />).instance();
            container.setState({ platformOptions: [{children: [{title: 'test', value: 1}], title: 'All Platform', value: '1'}] });

            // Act
            container.onChangePlatformSearch(value);

            // Assert
            expect(container.state.searchPlatformValue).toEqual('test');
        });
    });

    describe('handleSearchPlatform', () => {
        it('should set values exactly', () => {
            // Arrange 
            const value = 'test';
            const container = shallow(<EditNotification />).instance();
            container.refs = {
                form: {
                    state: {
                        values: {
                            platforms: [{ title: 'test', value: 1 }],
                            notificationTypeId: 1,
                            productId: 1,
                            channelIds: '1,2',
                            recipients: [{ id: 1, value: 'test' }]
                        }
                    },
                    setFieldValue: jest.fn()
                }
            };
            container.setState({
                platformOptions: [{children: [{title: 'test', value: 1}], title: 'All Platform', value: '1'}],
                notificationId: 1, notification: { notificationTypeId: 1 }
            });
            container.getRecipients = jest.fn();

            // Act
            container.handleSearchPlatform(value);

            // Assert
            expect(container.refs.form.setFieldValue).toHaveBeenCalledWith('platforms',[{title: 'test', value: 1}, {label: 'test', value: 1}]);
            expect(container.state.searchPlatformValue).toEqual('');
            expect(container.getRecipients).toHaveBeenCalledWith(1, 1, 1, '1,1');
            expect(container.refs.form.setFieldValue).toHaveBeenCalledWith('recipients', []);
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
                notificationTypeId: true,
                recipients: true
            },
            touched: {
                notificationTypeId: true,
                recipients: true
            },
            handleChange: jest.fn(),
            setFieldValue: jest.fn(),
            isSubmitting: jest.fn()
        };

        const state = {
            platformOptions: [{ title: 'test', value: 1 }],
            notificationId: 1, notification: { notificationTypeId: 1 },
            initialObj: {
                platforms: [{ title: 'test', value: 1 }],
                recipients: [{ id: 1, value: 'test' }]
            }
        };

        it('renderForm label exactly', () => {
            //Arrange
            const container = shallow(<EditNotification />).instance();
            container.setState(state);

            //Act
            const renderForm = shallow(container.renderForm({...params}));

            //Assert
            expect(renderForm.find(Label).length).toBe(9);
            expect(renderForm.find(FormFeedback).length).toBe(1);
        });

        it('renderForm ReactMde exactly', () => {
            //Arrange
            const container = shallow(<EditNotification />).instance();
            container.setState(state);

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
            const container = shallow(<EditNotification />).instance();
            container.setState(state);

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
            const container = shallow(<EditNotification />);
            container.instance().setState({
                notificationTypes: null,
                products: null
            });

            // Assert
            expect(container.type()).toEqual(null);
        });
    });
});