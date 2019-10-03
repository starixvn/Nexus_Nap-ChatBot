import { mappingStatus } from '../../../constants/constants';
import CommonFunctions from '../CommonFunctions';

describe('CommonFunctions', () => {
    describe('getMappingClass', () => {
        test.each`
            inputValue | expectedData
            ${mappingStatus.BotRemoved}  | ${'badge-grey'}
            ${mappingStatus.NotMapped}  | ${'badge-red'}
            ${mappingStatus.Mapped}  | ${'badge-green'}
            ${10000}  | ${''}
        `('return correct class name', ({ inputValue, expectedData }) => {
            // Act
            let result = CommonFunctions.getMappingClass(inputValue);

            // Assert
            expect(result).toBe(expectedData);
        });
    });

    describe('isBotRemoved', () => {
        test.each`
            inputValue | expectedData
            ${mappingStatus.BotRemoved}  | ${true}
            ${10000}  | ${false}
        `('is Bot Removed', ({ inputValue, expectedData }) => {
            // Act
            let result = CommonFunctions.isBotRemoved(inputValue);

            // Assert
            expect(result).toBe(expectedData);
        });
    });

    describe('isMapped', () => {
        test.each`
            inputValue | expectedData
            ${mappingStatus.Mapped}  | ${true}
            ${10000}  | ${false}
        `('is Mapped', ({ inputValue, expectedData }) => {
            // Act
            let result = CommonFunctions.isMapped(inputValue);

            // Assert
            expect(result).toBe(expectedData);
        });
    });

    describe('getLicenseeStatus', () => {
        test.each`
            inputValue | expectedData
            ${''}  | ${'N/A'}
            ${'xx'}  | ${'xx'}
        `('getLicenseeStatus', ({ inputValue, expectedData }) => {
            // Act
            let result = CommonFunctions.getLicenseeStatus(inputValue);

            // Assert
            expect(result).toBe(expectedData);
        });
    });

    describe('isEmptyObject', () => {
        test.each`
            inputValue | expectedData
            ${{}}  | ${true}
            ${{id: 1}}  | ${false}
        `('isEmptyObject', ({ inputValue, expectedData }) => {
                // Act
            let result = CommonFunctions.isEmptyObject(inputValue);

                // Assert
                expect(result).toBe(expectedData);
            });
    });
});