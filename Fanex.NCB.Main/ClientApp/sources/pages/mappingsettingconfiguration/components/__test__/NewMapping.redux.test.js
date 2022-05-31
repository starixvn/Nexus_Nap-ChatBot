jest.mock('react-redux');
jest.mock('redux');
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

describe('connect', () => {
    let mapStateToProps, mapDispatchToProps;
    const connectMock = jest.fn();

    beforeAll(() => {
        connect.mockImplementation((mapStateToPropsFunc, mapDispatchToPropsFunc) => {
            mapStateToProps = mapStateToPropsFunc;
            mapDispatchToProps = mapDispatchToPropsFunc;
            return connectMock;
        });
        require('../NewMapping').default;
    });

    afterAll(() => {
        jest.clearAllMocks();
    });

    it('props should be mapped correctly', () => {
        // Arrange
        const expected = {
            createMappingResult: { isSuccessful: true },
            notificationTypes: 1,
            msgContentsTypes: 1,
            notificationTypeRef: { id: 1, name: 'test' },
            jobInfo: { id: 1, name: 'test' },
            settingTypes: [{ id: 1, name: 'Yup' }]
        };
        const state = {
            createMappingResult: { isSuccessful: true },
            notificationTypes: 1,
            msgContentsTypes: 1,
            notificationTypeRef: { id: 1, name: 'test' },
            jobInfo: { id: 1, name: 'test' },
            settingTypes: [{ id: 1, name: 'Yup' }]
        };

        // Act
        const actual = mapStateToProps(state);

        // Assert
        expect(actual).toEqual(expected);
    });

    it('bindActionCreators should be called with correct parameter', () => {
        // Arrange
        const mockBindAction = jest.fn();
        bindActionCreators.mockImplementationOnce(mockBindAction);
        const dispatch = {};

        // Act
        mapDispatchToProps(dispatch);

        // Assert
        expect(mockBindAction).toHaveBeenCalled();
    });
});